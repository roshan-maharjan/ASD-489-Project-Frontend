// In Services/CurrentUserService.cs
using ExpenseSplit.Models.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims; // Make sure to add this

namespace ExpenseSplit.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly AuthenticationStateProvider _authStateProvider;

        public CurrentUserService(AuthenticationStateProvider authStateProvider)
        {
            _authStateProvider = authStateProvider;
        }

        public async Task<string?> GetUserIdAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                // FindFirst is a little more direct than .Claims.FirstOrDefault(...)
                var userIdClaim = user.FindFirst("nameid");
                return userIdClaim?.Value;
            }

            return null; // User is not logged in or has no ID claim
        }

        public async Task<UserProfile?> GetUserProfileAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                var id = user.FindFirst("nameid")?.Value;
                var firstName = user.FindFirst("given_name")?.Value;
                var lastName = user.FindFirst("family_name")?.Value;
                var email = user.FindFirst("email")?.Value;

                return new UserProfile
                {
                    Id = id,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email
                };
            }

            return null;
        }
    }
}