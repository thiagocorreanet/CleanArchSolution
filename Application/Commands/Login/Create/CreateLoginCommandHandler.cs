
using Application.Notification;

using Core.Repositories;

using MediatR;

namespace Application.Commands.Login.Create;

public class CreateLoginCommandHandler : CreateBaseCommand, IRequestHandler<CreateLoginCommandRequest, CreateLoginCommandResponse>
{
    readonly IAuthRepository _authRepository;

    public CreateLoginCommandHandler(INotificationError notificationError, IAuthRepository authRepository) : base(notificationError)
    {
        _authRepository = authRepository;
    }

    public async Task<CreateLoginCommandResponse?> Handle(CreateLoginCommandRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authRepository.AuthenticateAsync(username: request.Email, password: request.Passwork);

            if (result is null)
            {
                Notify(message: "Authentication failed, please try again.");
                return null;
            }

            var claims = new List<CreateClaimUserCommandResponse>();
            foreach (var item in result.UserToken.Claims)
            {
                claims.Add(new CreateClaimUserCommandResponse
                {
                    Type = item.Type,
                    Value = item.Value
                });
            }

            var response = new CreateLoginCommandResponse
            {
                AccessToken = result.AccessToken,
                ExpiresIn = result.ExpiresIn,
                UserToken = new CreateUserTokenCommandResponse
                {
                    Id = result.UserToken.Id,
                    Email = result.UserToken.Email,
                    Claims = claims
                }
            };

            return response;
        }
        catch (Exception)
        {
            // TODO: Implementar serilog
            Notify(message: "Oops! We were unable to process your request.");

        }

        return null;
    }
}
