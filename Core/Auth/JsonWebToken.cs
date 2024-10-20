namespace Core.Auth;

public class JsonWebToken
{
    public string Secret { get; set; } = null!;
    public int ExpirationHours { get; set; }
    public string Issuer { get; set; } = null!;
    public string ValidIn { get; set; } = null!;
}

public class UserToken
{
    public string? Id { get; set; }
    public string? Email { get; set; }
    public IEnumerable<ClaimUser> Claims { get; set; }
}

public class LoginUser
{
    public string? AccessToken { get; set; }
    public double ExpiresIn { get; set; }
    public UserToken? UserToken { get; set; }
}

public class ClaimUser
{
    public string? Value { get; set; }
    public string? Type { get; set; }
}
