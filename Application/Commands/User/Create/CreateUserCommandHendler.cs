using Application.Notification;

using Core.Repositories;

using MediatR;

namespace Application.Commands.User.Create;

public class CreateUserCommandHendler : CreateBaseCommand, IRequestHandler<CreateUserCommandRequest, bool>
{
    readonly IAuthRepository _authRepository;

    public CreateUserCommandHendler(INotificationError notificationError, IAuthRepository authRepository) : base(notificationError)
    {
        _authRepository = authRepository;
    }

    public async Task<bool> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
    {
        try
        {

            bool result = await _authRepository.RegisterUserAsync(userName: request.Email, password: request.Password);

            if (!result)
            {
                Notify("Oops! We were unable to register your user, please try again.");
                return false;
            }
        }
        catch (Exception)
        {
            Notify("Oops! We were unable to process your request.");
        }

        return true;
    }
}
