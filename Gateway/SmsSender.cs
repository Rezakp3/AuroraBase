using static Ghasedak.Core.Models.Results;

namespace Gateway;

public class SmsSender(Core.ViewModel.Base.Ghasedak ghasedak) : Ghasedak.Core.Api(ghasedak.ApiKey)
{
    public Task<SendResult> SendOtp(string PhoneNumber, string Code) 
        => VerifyAsync(1, ghasedak.Otp, [PhoneNumber], Code);
}
