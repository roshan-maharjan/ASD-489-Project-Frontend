using ExpenseSplit.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace ExpenseSplit.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly IConfiguration _config;
        private readonly string _registrationRoute;
        private readonly string _loginRoute;

        public AuthService(HttpClient httpClient,
                           AuthenticationStateProvider authStateProvider,
                           IConfiguration config)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
            _config = config;

            // Get configurable API routes
            _registrationRoute = _config.GetSection("Routes:Registration").Value ?? "/accounts/registration";
            _loginRoute = _config.GetSection("Routes:Login").Value ?? "/accounts/login";
        }

        public async Task<AuthResponseDto> RegisterUser(UserForRegistrationDto userForRegistration)
        {
            var result = await _httpClient.PostAsJsonAsync(_registrationRoute, userForRegistration);

            if (result.IsSuccessStatusCode)
            {
                // Registration success typically returns 201 Created or 200 OK.
                return new AuthResponseDto { IsSuccessful = true };
            }
            else
            {
                // Assuming your API returns error messages
                var errorContent = await result.Content.ReadAsStringAsync();
                return new AuthResponseDto { IsSuccessful = false, ErrorMessage = $"Registration failed: {errorContent}" };
            }
        }

        public async Task<AuthResponseDto> Login(UserForLoginDto userForLogin)
        {
            var result = await _httpClient.PostAsJsonAsync(_loginRoute, userForLogin);

            if (result.IsSuccessStatusCode)
            {
                var authResponse = await result.Content.ReadFromJsonAsync<AuthResponseDto>();

                if (authResponse?.Token != null)
                {
                    // Inform the custom state provider that the user has logged in
                    await ((CustomAuthStateProvider)_authStateProvider).MarkUserAsAuthenticated(authResponse.Token);
                    return new AuthResponseDto { IsSuccessful = true };
                }
                return new AuthResponseDto { IsSuccessful = false, ErrorMessage = "Login successful, but no token received." };
            }
            else
            {
                var errorContent = await result.Content.ReadAsStringAsync();
                return new AuthResponseDto { IsSuccessful = false, ErrorMessage = $"Login failed: {errorContent}" };
            }
        }

        public async Task Logout()
        {
            await ((CustomAuthStateProvider)_authStateProvider).MarkUserAsLoggedOut();
        }
    }
}
