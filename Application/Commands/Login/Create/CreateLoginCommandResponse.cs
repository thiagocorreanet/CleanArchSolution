namespace Application.Commands.Login.Create;

public class CreateUserTokenCommandResponse
{
    public string? Id { get; set; }
    public string? Email { get; set; }
    public IEnumerable<CreateClaimUserCommandResponse> Claims { get; set; }
}

public class CreateLoginCommandResponse
{
    public string? AccessToken { get; set; }
    public double ExpiresIn { get; set; }
    public CreateUserTokenCommandResponse? UserToken { get; set; }
}

public class CreateClaimUserCommandResponse
{
    public string? Value { get; set; }
    public string? Type { get; set; }
}
