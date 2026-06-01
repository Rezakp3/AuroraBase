namespace Infrastructure.ExternalService.SmsService;

public class SmsSettings
{
    public string ApiToken { get; set; }
    public int OtpTemplateId { get; set; }
    public string OtpParameter { get; set; }
}
