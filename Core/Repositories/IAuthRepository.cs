using Core.Auth;

using Microsoft.AspNetCore.Identity;

namespace Core.Repositories;

public interface IAuthRepository
{
    Task<SignInResult?> AuthenticateAsync(string username, string password, CancellationToken cancellationToken);
    Task<IdentityResult> RegisterUserAsync(string userName, string password, CancellationToken cancellationToken);
    Task LogoutAsync();
}
