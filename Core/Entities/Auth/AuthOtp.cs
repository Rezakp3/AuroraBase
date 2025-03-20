using System.Reflection.Metadata;

namespace Core.Entities.Auth;

public class AuthOtp : Auth
{
    public int? LoginCode { get; set; }
    public int LoginCodeTryCount { get; set; }
    public DateTime? LoginCodeExpirationDate { get; set; }
}
