using ExpenseSplit.Models;

namespace ExpenseSplit.Services.AuthService
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterUser(UserForRegistrationDto userForRegistration);
        Task<AuthResponseDto> Login(UserForLoginDto userForLogin);
        Task Logout();

    }
}
