// In Services/ICurrentUserService.cs
using ExpenseSplit.Models.DTOs;

namespace ExpenseSplit.Services
{
    public interface ICurrentUserService
    {
        Task<string?> GetUserIdAsync();
        Task<UserProfile?> GetUserProfileAsync();
    }
}