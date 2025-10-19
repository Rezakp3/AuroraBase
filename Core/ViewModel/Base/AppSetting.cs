namespace Core.ViewModel.Base;

public class AppSetting
{
    public string AllowedHosts { get; set; }
    public Connectionstrings ConnectionStrings { get; set; }
}

public class Connectionstrings
{
    public string DefaultConnection { get; set; }
}
