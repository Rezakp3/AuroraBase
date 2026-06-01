using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.Repositories;
using Aurora.Jwt.Models;
using IPE.SmsIrClient;
using IPE.SmsIrClient.Models.Requests;
using IPE.SmsIrClient.Models.Results;

namespace Infrastructure.ExternalService.SmsService;

public class SmsIrService(ISettingRepository setting) : ISmsService
{

    public async Task<bool> SendOtp(string PhoneNumber, string OtpCode)
    {
        try
        {
            var smsSetting = await setting.GetByGroupAsync<SmsSettings>("SmsSettings");
            SmsIr smsIr = new(smsSetting.ApiToken);

            VerifySendParameter[] verifySendParameters = [new(smsSetting.OtpParameter, OtpCode)];

            var response = await smsIr.VerifySendAsync(PhoneNumber, smsSetting.OtpTemplateId, verifySendParameters);

            VerifySendResult sendResult = response.Data;
            int messageId = sendResult.MessageId;
            decimal cost = sendResult.Cost;
            return true;
        }
        catch (Exception ex) 
        {
            string errorName = ex.GetType().Name;
            string errorNameDescription = errorName switch
            {
                "UnauthorizedException" => "The provided token is not valid or access is denied.",
                "LogicalException" => "Please check and correct the request parameters.",
                "TooManyRequestException" => "The request count has exceeded the allowed limit.",
                "UnexpectedException" or "InvalidOperationException" => "An unexpected error occurred on the remote server.",
                _ => "Unable to send the request due to an unspecified error.",
            };

            string errorMessage = ex.Message;
            string errorDescription = "There is a problem with the request." +
                $"\n - Error: {errorName} - {errorNameDescription} - {errorMessage}";
            throw new Exception(errorDescription);
        }
    }
}
