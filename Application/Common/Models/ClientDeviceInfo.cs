namespace Application.Common.Models;
public class ClientDeviceInfo
{
    public string IpAddress { get; set; }
    public string Browser { get; set; }      // Chrome, Firefox
    public string BrowserVersion { get; set; }
    public string OperatingSystem { get; set; } // Windows, Android, iOS
    public string DeviceType { get; set; }   // Desktop, Mobile, Tablet
    public string RawUserAgent { get; set; } 
}