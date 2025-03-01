namespace Core.ViewModel.Base;

public class AppSetting
{
    public string AllowedHosts { get; set; }
    public Connectionstrings ConnectionStrings { get; set; }
    public Ghasedak Ghasedak { get; set; }
}

public class Connectionstrings
{
    public string DefaultConnection { get; set; }
}

public class Ghasedak
{
    public string ApiKey { get; set; }
    public string Otp { get; set; }
}