namespace Core.ViewModel.AuthVm;

public class LoginDto
{
    public string? AccessToken { get; set; }
    public Guid RefreshToken { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }
}
