namespace Core.Entities.Auth;

public class AuthPassword : Auth
{
    public string? Password { get; set; }
    public string? Salt { get; set; }
}
