
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Application.Notification;

using Core.Auth;
using Core.Repositories;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Commands.Login.Create;

public class CreateLoginCommandHandler : CreateBaseCommand, IRequestHandler<CreateLoginCommandRequest, CreateLoginCommandResponse>
{
    readonly IAuthRepository _authRepository;
    readonly UserManager<IdentityUser> _userManager;
    readonly JsonWebToken _jsonWebToken;
    readonly ILogger<CreateLoginCommandHandler> _logger;

    public CreateLoginCommandHandler(INotificationError notificationError, IAuthRepository authRepository, UserManager<IdentityUser> userManager, IOptions<JsonWebToken> jsonWebToken, ILogger<CreateLoginCommandHandler> logger) : base(notificationError)
    {
        _authRepository = authRepository;
        _userManager = userManager;
        _jsonWebToken = jsonWebToken.Value;
        _logger = logger;
    }

    public async Task<CreateLoginCommandResponse?> Handle(CreateLoginCommandRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authRepository.AuthenticateAsync(username: request.Email, password: request.Passwork, cancellationToken: cancellationToken);

            if (result is null || !result.Succeeded)
            {
                _logger.LogError($"Authentication failed, please try again. Handler: {nameof(CreateLoginCommandResponse)} - Result login: {result?.Succeeded}");
                Notify(message: "Authentication failed, please try again.");
                return null;
            }

            return await (GenerateJwtTokenAsync(request.Email));
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"ops! We were unable to process your request. Details error: {ex.Message}");
            Notify(message: "Oops! We were unable to process your request.");

        }

        return null;
    }

    private async Task<CreateLoginCommandResponse> GenerateJwtTokenAsync(string userName)
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

        var response = new CreateLoginCommandResponse
        {
            AccessToken = encodedToken,
            ExpiresIn = TimeSpan.FromHours(_jsonWebToken.ExpirationHours).TotalMicroseconds,
            UserToken = new CreateUserTokenCommandResponse
            {
                Id = user.Id,
                Email = user.Email,
                Claims = claims.Select(x => new CreateClaimUserCommandResponse { Type = x.Type, Value = x.Value }),

            }
        };

        return response;
    }

    private static long ToUnixEpochDate(DateTime date)
        => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalMicroseconds);
}
