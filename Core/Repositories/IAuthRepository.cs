using Core.Auth;

namespace Core.Repositories;

public interface IAuthRepository
{
    Task<LoginUser> AuthenticateAsync(string username, string password);
    Task<bool> RegisterUserAsync(string userName, string password);
    Task LogoutAsync();
    Task<LoginUser> GenerateJwtTokenAsync(string userName);
}
