namespace Application.Common.Interfaces.ExternalServices;

public interface ISmsService 
{
    Task<bool> SendOtp(string PhoneNumber, string OtpCode);
}