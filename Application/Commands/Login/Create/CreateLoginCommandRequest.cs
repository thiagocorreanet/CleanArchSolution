using MediatR;

namespace Application.Commands.Login.Create;

public class CreateLoginCommandRequest : IRequest<CreateLoginCommandResponse>
{
    public string Email { get; set; } = null!;
    public string Passwork { get; set; } = null!;

}
