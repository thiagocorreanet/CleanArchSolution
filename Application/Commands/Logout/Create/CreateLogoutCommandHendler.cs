
using Application.Commands.Login.Create;
using Application.Notification;

using Core.Repositories;
using MediatR;

using Microsoft.Extensions.Logging;

namespace Application.Commands.Logout.Create;

public class CreateLogoutCommandHendler : CreateBaseCommand, IRequestHandler<CreateLogoutCommandRequest, bool>
{
    readonly IAuthRepository _authRepository;
    readonly ILogger<CreateLoginCommandHandler> _logger;

    public CreateLogoutCommandHendler(INotificationError notificationError, IAuthRepository authRepository, ILogger<CreateLoginCommandHandler> logger) : base(notificationError)
    {
        this._authRepository = authRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(CreateLogoutCommandRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _authRepository.LogoutAsync();
        }
        catch (Exception ex)
        {
            _logger.LogCritical(message: $"ops! We were unable to process your request. Details error: {ex.Message}");
            Notify(message: "Oops! We were unable to process your request.");
        }

        return true;
    }
}
