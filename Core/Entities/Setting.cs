using Core.Enums;

namespace Core.Entities;

public class Setting : BaseEntity<int>
{
    public string Group { get; set; }              // "RabbitMQ", "Redis", "Smtp"
    public string Key { get; set; }                // "Host", "Port", "Password"
    public string Value { get; set; }              // مقدار (همیشه string در DB)
    public string? Description { get; set; }       // توضیحات
    public ESettingDataType DataType { get; set; } // نوع داده برای validation
    public bool IsEncrypted { get; set; }          // رمزنگاری شده؟
}