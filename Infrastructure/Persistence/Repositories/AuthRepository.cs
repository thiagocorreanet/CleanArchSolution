
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

    public async Task<LoginUser?> AuthenticateAsync(string username, string password)
    {
        var userFindByEmail = await _userManager.FindByEmailAsync(username);

        if (userFindByEmail is null) 
            return null;

        var result = await _signInManager.PasswordSignInAsync(userName: username, password: password, false, false);

        if (!result.Succeeded)
            return null;

        return await GenerateJwtTokenAsync(userName: username);
    }

    public async Task<LoginUser> GenerateJwtTokenAsync(string userName)
    {
        var user = await _userManager.FindByEmailAsync(userName);
        var claims = await _userManager.GetClaimsAsync(user);
        var userRoles = await _userManager.GetRolesAsync(user);

        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
        claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
        foreach (var role in userRoles)
            claims.Add(new Claim("role", role));

        var identityClaims = new ClaimsIdentity();
        identityClaims.AddClaims(claims);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jsonWebToken.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = identityClaims,
            Issuer = _jsonWebToken.Issuer,
            Audience = _jsonWebToken.ValidIn,
            Expires = DateTime.UtcNow.AddHours(_jsonWebToken.ExpirationHours),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var encodedToken = tokenHandler.WriteToken(token);

        var response = new LoginUser
        {
            AccessToken = encodedToken,
            ExpiresIn = TimeSpan.FromHours(_jsonWebToken.ExpirationHours).TotalMicroseconds,
            UserToken = new UserToken
            {
                Id = user.Id,
                Email = user.Email,
                Claims = claims.Select(x => new ClaimUser { Type = x.Type, Value = x.Value }),

            }
        };

        return response;
    }

    public async Task LogoutAsync()
       => await _signInManager.SignOutAsync();

    public async Task<bool> RegisterUserAsync(string userName, string password)
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
            return false;
        }
        else
        {
            await _signInManager.SignInAsync(user, false);
        }

        return result.Succeeded;
    }

    private static long ToUnixEpochDate(DateTime date)
        => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalMicroseconds);
}
