using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace ExpenseSplit.Services.AuthService
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _http;
        private static readonly string TokenKey = "authToken";

        public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient http)
        {
            _localStorage = localStorage;
            _http = http;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string? savedToken = await _localStorage.GetItemAsStringAsync(TokenKey);

            if (string.IsNullOrWhiteSpace(savedToken))
            {
                // No token, user is anonymous
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // --- Add this try-catch block ---
            try
            {
                var claims = ParseClaimsFromJwt(savedToken);

                // Check for expiration
                var expClaim = claims.FirstOrDefault(c => c.Type.Equals("exp", StringComparison.OrdinalIgnoreCase));
                if (expClaim != null)
                {
                    var expTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim.Value));
                    if (expTime.UtcDateTime <= DateTime.UtcNow)
                    {
                        // Token is expired, log them out
                        await MarkUserAsLoggedOut(); // Use your existing logout logic
                        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                    }
                }

                // Token is valid and not expired
                var identity = new ClaimsIdentity(claims, "jwt");
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            catch (Exception)
            {
                // Token is malformed or invalid
                await MarkUserAsLoggedOut(); // Clean up bad token
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public async Task MarkUserAsAuthenticated(string token)
        {
            await _localStorage.SetItemAsStringAsync(TokenKey, token);
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
            var authState = Task.Run(() => new AuthenticationState(authenticatedUser));

            // Update HttpClient default header
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            NotifyAuthenticationStateChanged(authState);
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _localStorage.RemoveItemAsync(TokenKey);
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));

            // Clear HttpClient default header
            _http.DefaultRequestHeaders.Authorization = null;

            NotifyAuthenticationStateChanged(authState);
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];

            var jsonBytes = ParseBase64WithoutPadding(payload);

            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs != null)
            {
                // This is a more robust way to handle claims that might be arrays (like roles)
                keyValuePairs.TryGetValue(ClaimTypes.Role, out object? roles);

                if (roles != null)
                {
                    if (roles.ToString().Trim().StartsWith("["))
                    {
                        var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());
                        if (parsedRoles != null)
                        {
                            foreach (var parsedRole in parsedRoles)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                            }
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                    }
                    keyValuePairs.Remove(ClaimTypes.Role);
                }

                // Add remaining claims
                claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty)));
            }

            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            // Replace URL-safe characters
            base64 = base64.Replace('-', '+').Replace('_', '/');

            // Add back padding
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            // Now it's valid standard Base-64
            return Convert.FromBase64String(base64);
        }
    }
}
