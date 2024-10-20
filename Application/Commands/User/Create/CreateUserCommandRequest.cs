using MediatR;

namespace Application.Commands.User.Create;

public class CreateUserCommandRequest : IRequest<bool>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
