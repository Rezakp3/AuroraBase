namespace EmailSender.ViewModel;

public class SendEmailVm
{
    public string Reciever { get; set; }
    public string Subject { get; set; }
    public string TextBody { get; set; }
    public string HtmlBody { get; set; }
}
