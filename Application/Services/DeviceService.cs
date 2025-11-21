using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Microsoft.AspNetCore.Http;
using UAParser;

namespace Application.Services;

public class DeviceService(IHttpContextAccessor _httpContextAccessor) : IDeviceService
{
    // کش کردن پارسر برای پرفورمنس (چون دیتابیس Regex سنگینی دارد)
    private static readonly Parser _uaParser = Parser.GetDefault();

    public ClientDeviceInfo GetClientInfo()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return new ClientDeviceInfo();

        // 1. دریافت User-Agent
        var userAgent = context.Request.Headers.UserAgent.ToString();
        if (string.IsNullOrEmpty(userAgent)) userAgent = "Unknown";

        // 2. پارس کردن توسط کتابخانه UAParser
        var clientInfo = _uaParser.Parse(userAgent);

        return new ClientDeviceInfo
        {
            IpAddress = GetIpAddress(),
            RawUserAgent = userAgent,

            // استخراج اطلاعات
            Browser = clientInfo.UA.Family, // e.g. Chrome
            BrowserVersion = $"{clientInfo.UA.Major}.{clientInfo.UA.Minor}",
            OperatingSystem = $"{clientInfo.OS.Family} {clientInfo.OS.Major}", // e.g. Windows 10

            // تشخیص نوع دستگاه (ساده)
            DeviceType = IsMobile(userAgent) ? "Mobile" : "Desktop"
        };
    }

    public string GetIpAddress()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return "0.0.0.0";

        // نکته حیاتی: اگر پشت Nginx یا Cloudflare هستید، IP واقعی در هدر X-Forwarded-For است
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedIp))
        {
            // ممکن است چند IP باشد (Chain)، اولی IP کلاینت است
            return forwardedIp.FirstOrDefault()?.Split(',')[0].Trim() ?? "0.0.0.0";
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
    }

    // یک متد ساده برای تشخیص موبایل (UAParser هم Device.Family دارد اما گاهی Generic است)
    private static bool IsMobile(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent)) return false;
        return userAgent.Contains("Mobi", StringComparison.OrdinalIgnoreCase) ||
               userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase) ||
               userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase);
    }
}