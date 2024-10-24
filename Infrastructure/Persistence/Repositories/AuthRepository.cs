
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Core.Auth;
using Core.Repositories;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Persistence.Repositories;

public class AuthRepository : IAuthRepository
{
    readonly UserManager<IdentityUser> _userManager;
    readonly SignInManager<IdentityUser> _signInManager;
    readonly RoleManager<IdentityRole> _roleManager;
    readonly IConfiguration _iConfiguration;
    readonly JsonWebToken _jsonWebToken;

    public AuthRepository(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration iConfiguration, IOptions<JsonWebToken> jsonWebToken)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _iConfiguration = iConfiguration;
        _jsonWebToken = jsonWebToken.Value; 
    }

    public async Task<SignInResult?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken)
    {
        var result = await _signInManager.PasswordSignInAsync(
            userName: email,
            password: password,
            isPersistent: false,
            lockoutOnFailure: false);

        cancellationToken.ThrowIfCancellationRequested();

        return result;
    }

    public async Task<IdentityResult> RegisterUserAsync(string userName, string password, CancellationToken cancellationToken)
    {

        var user = new IdentityUser()
        {
            Email = userName,
            UserName = userName,
            EmailConfirmed = true
        };

        var result  = await _userManager.CreateAsync(user: user, password: password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Failed to create user: {errors}");
        }

        await _signInManager.SignInAsync(user, isPersistent: false);

        cancellationToken.ThrowIfCancellationRequested();

        return result;
    }

    public async Task LogoutAsync()
       => await _signInManager.SignOutAsync();
}
