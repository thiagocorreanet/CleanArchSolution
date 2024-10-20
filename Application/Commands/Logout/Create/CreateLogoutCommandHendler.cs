
using Application.Notification;

using Core.Repositories;

using MediatR;

namespace Application.Commands.Logout.Create;

public class CreateLogoutCommandHendler : CreateBaseCommand, IRequestHandler<CreateLogoutCommandRequest, bool>
{
    readonly IAuthRepository _authRepository;

    public CreateLogoutCommandHendler(INotificationError notificationError, IAuthRepository authRepository) : base(notificationError)
    {
        this._authRepository = authRepository;
    }

    public async Task<bool> Handle(CreateLogoutCommandRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _authRepository.LogoutAsync();
        }
        catch (Exception)
        {
            Notify(message: "Oops! We were unable to process your request.");
        }

        return true;
    }
}
