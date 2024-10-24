using Application.Commands.Login.Create;
using Application.Notification;

using Core.Repositories;
using MediatR;

using Microsoft.Extensions.Logging;

namespace Application.Commands.User.Create;

public class CreateUserCommandHendler : CreateBaseCommand, IRequestHandler<CreateUserCommandRequest, bool>
{
    readonly IAuthRepository _authRepository;
    readonly ILogger<CreateLoginCommandHandler> _logger;

    public CreateUserCommandHendler(INotificationError notificationError, IAuthRepository authRepository, ILogger<CreateLoginCommandHandler> logger) : base(notificationError)
    {
        _authRepository = authRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
    {
        try
        {

            var result = await _authRepository.RegisterUserAsync(userName: request.Email, password: request.Password, cancellationToken: cancellationToken);

            if (!result.Succeeded || result is null)
            {
                _logger.LogError(message: $"Oops! We were unable to register your user, please try again. {result?.Succeeded}");
                Notify("Oops! We were unable to register your user, please try again.");
                return false;
            }

            if (result is null || !result.Succeeded)
            {
                var errors = result?.Errors.Select(e => e.Description).ToList() ?? new List<string> { "Unknown error." };
                _logger.LogError(message: $"Failed to register user: {errors}", args: string.Join(", ", errors));
                Notify("Sorry, we were unable to register your username. Please try again.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(message: $"ops! We were unable to process your request. Details error: {ex.Message}");
            Notify(message: "Oops! We were unable to process your request.");
        }

        return true;
    }
}
