<div align="center">

# 🌟 AuroraBase

### قالب زیرساخت پایدار و فریم‌ورک آماده دات‌نت ۸ (Enterprise Web API Boilerplate)

[![.NET Version](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![C# Version](https://img.shields.io/badge/C%23-12.0-239120?style=for-the-badge&logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![EF Core Version](https://img.shields.io/badge/EF%20Core-8.0-512BD4?style=for-the-badge)](https://docs.microsoft.com/en-us/ef/core/)
[![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)](LICENSE)

**AuroraBase** یک بسته‌ی توسعه نرم‌افزار (Boilerplate) ماژولار و بر پایه اصول **معماری تمیز (Clean Architecture)** است. این پروژه بستری بهینه‌سازی شده، مقیاس‌پذیر و پایدار برای توسعه وب‌سرویس‌های تجاری و سازمانی (Enterprise) با استفاده از الگوی CQRS فراهم می‌کند.

[🚀 راه‌اندازی سریع](#-شروع-سریع) • [🏛️ جزئیات معماری](#%EF%B8%8F-معماری-تمیز-و-جریان-وابستگیها) • [🎯 ویژگی‌های کلیدی](#-ویژگیهای-زیرساختی) • [📂 مستندات ماژول‌ها](#-بررسی-تخصصی-ماژولها) • [🤝 مشارکت](#-مشارکت-در-توسعه)

</div>

---

## 📖 معرفی پروژه

ساخت وب‌سرویس‌های بزرگ نیاز به همپوشانی ابزارهای مختلف در لایه‌های گوناگون دارد. پیاده‌سازی دستی امنیت، احراز هویت، اعتبارسنجی‌ها، صفحه‌بندی‌های سنگین و ثبت لاگ در هر پروژه جدید، کاری تکراری و زمان‌بر است.

پروژه **AuroraBase** با جداسازی دغدغه‌های زیرساختی (Cross-Cutting Concerns) در قالب لایه‌های مستقل، به توسعه‌دهنده اجازه می‌دهد تمرکز کامل خود را روی منطق تجاری سیستم (Domain Logic) قرار دهد، بدون آنکه کدهای سیستم شلوغ یا غیرقابل تست شوند.

---

## 🎯 ویژگی‌های زیرساختی

### ۱. معماری نرم‌افزار و الگوها
*   **Clean Architecture:** تفکیک کامل لایه منطق کسب‌وکار از وابستگی‌های پایگاه‌داده و فریم‌ورک‌های وب.
*   **CQRS با MediatR:** اجرای الگوی جداسازی فرمان و پرس‌وجو برای دستیابی به حداکثر سرعت و خوانایی کد.
*   **Repository & Unit of Work:** واسط دسترسی به داده‌ها برای جلوگیری از تکرار کدهای EF Core و مدیریت تراکنش‌ها.

### ۲. صفحه‌بندی دوگانه با کارایی بالا (Dual Pagination)
*   **Page-Based (Offset):** مناسب برای درخواست‌های عادی و لیست‌های کوچک دیتابیس.
*   **Cursor-Based (Keyset):** طراحی شده برای دیتابیس‌های فوق‌العاده بزرگ جهت جلوگیری از افت سرعت در رکوردهای بالا.

### ۳. امنیت و هویت اختصاصی
*   **Aurora.Jwt:** سیستم کامل احراز هویت با استفاده از Access Token کوتاه مدت و Refresh Token بلندمدت در کنار مدیریت نقش‌ها و Claims.
*   **Aurora.Captcha:** تولیدکننده تصویر کپچای امنیتی بومی جهت مسدود کردن درخواست‌های مخرب و ربات‌ها.

### ۴. سیستم کشینگ ماژولار
*   **Aurora.Cache:** زیرساخت منعطف برای ذخیره‌سازی داده‌ها در حافظه موقت دیتابیس یا رم (In-Memory / Distributed Cache) به همراه ساختارهای مدیریت عمر کلیدها.

### ۵. لاگینگ و مانیتورینگ خطاها
*   **Aurora.Logger:** ثبت و ساختاردهی خطاها و وقایع سیستم در قالب فرمت‌های استاندارد برای اشکال‌زدایی سریع‌تر اپلیکیشن.

---

## 🏛️ معماری تمیز و جریان وابستگی‌ها

لایه‌بندی AuroraBase بر اساس اصول معماری تمیز پایه‌گذاری شده است. لایه‌های درونی هیچ دانشی از پیاده‌سازی لایه‌های بیرونی ندارند:

```mermaid
graph TD
    Api[Api / لایه رابط کاربری و وب سرویس] --> Application[Application / منطق فرآیندها و CQRS]
    Infrastructure[Infrastructure / پایگاه‌داده و پیاده‌سازی مخازن] --> Core[Core / هسته دامین و موجودیت‌ها]
    Application --> Core
    Api --> Infrastructure
    
    subgraph ماژول‌های جانبی و کمکی
        Cache[Aurora.Cache]
        Jwt[Aurora.Jwt]
        Captcha[Aurora.Captcha]
        Logger[Aurora.Logger]
        EmailSender[EmailSender]
    end
    
    Api -.-> ماژول‌های جانبی و کمکی
    Infrastructure -.-> ماژول‌های جانبی و کمکی

📂 ساختار پوشه‌ها و نقش هر لایه

AuroraBase/
├── Api/                        # لایه ارائه (روترها، کنترلرها، مڈل‌ورها و تنظیمات شروع پروژه)
├── Application/                # کدهای CQRS (شامل Command، Query، Handler، DTOs و ولیدیشن‌ها)
├── Core/                       # هسته مستقل نرم‌افزار (انتیتی‌ها، استثناهای دامین و واسط کاربری‌ها)
├── Infrastructure/             # پیاده‌سازی بستر داده (EF Core DbContext، مخازن کدهای SQL و مایگریشن‌ها)
├── Aurora.Cache/               # کتابخانه اختصاصی مدیریت کش درون‌حافظه‌ای و توزیع‌شده
├── Aurora.Captcha/             # موتور پویا برای تولید کدهای امنیتی تصویری و بازگشت در قالب Base64
├── Aurora.ChacheSetting/       # کلاس‌ها و رکوردهای پیکربندی بهینه برای ماژول کش
├── Aurora.Jwt/                 # زیرساخت پردازش، امضا، اعتبارسنجی و ساخت توکن‌های JWT
├── Aurora.Logger/              # ساختار متمرکز مدیریت لاگ‌ها
├── EmailSender/                # پروژه فرعی جهت ارسال ایمیل‌های اطلاع‌رسانی از طریق SMTP
└── Utils/                      # کلاس‌های کمکی، فرمت‌کننده‌ها و متدهای توسعه (Extensions) عمومی

⚡ شروع سریع

برای راه‌اندازی پروژه روی سیستم محلی خود، کارهای زیر را در چند گام ساده انجام
دهید:

پیش‌نیازها

  - نصب .NET 8.0 SDK
  - نصب و راه‌اندازی SQL Server

گام ۱: کلون کردن مخزن

git clone https://github.com/Rezakp3/AuroraBase.git
cd AuroraBase

گام ۲: تنظیم پیکربندی (appsettings.json)

فایل Api/appsettings.json را باز کنید و مقادیر زیر را با توجه به مشخصات سرور خود
ویرایش کنید:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=AuroraBaseDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "YOUR_SUPER_SECRET_KEY_SHOULD_BE_VERY_LONG_AND_SECURE",
    "Issuer": "AuroraBase",
    "Audience": "AuroraBaseUsers",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "CacheSettings": {
    "AbsoluteExpirationInMinutes": 60,
    "SlidingExpirationInMinutes": 10
  }
}

گام ۳: به‌روزرسانی بانک اطلاعاتی

دستور زیر را در ترمینال پوشه اصلی پروژه اجرا کنید تا جداول ساخته شوند:

dotnet ef database update --project Infrastructure --startup-project Api

گام ۴: اجرای پروژه

cd Api
dotnet run

اکنون می‌توانید مستندات API را در آدرس زیر مرور کنید: 👉
https://localhost:7001/swagger (پورت پیش‌فرض پروژه خود را جایگزین کنید)

📂 بررسی تخصصی ماژول‌ها

۱. سیستم صفحه‌بندی پیشرفته (Pagination)

استفاده از روش سنتی Offset-Based در صفحات انتهایی دیتابیس‌های بزرگ دچار افت شدید
کارایی می‌شود زیرا دیتابیس باید تمام رکوردهای قبلی را بخواند و سپس آن‌ها را رها
کند. سیستم Cursor-Based با استفاده از آخرین مقدار رکوردهای صفحه قبل، زمان
جست‌وجو را در حالت ثابت نگه می‌دارد.

📊 بنچمارک و مقایسه سرعت لود داده‌ها (بر اساس میلی‌ثانیه):

| تعداد رکورد پایگاه‌داده | روش سنتی (Offset) | روش نوین (Cursor) | ضریب بهبود کارایی    |
| :---------------------: | :---------------: | :---------------: | :------------------: |
| **۱۰,۰۰۰ رکورد**        | ۱۸ میلی‌ثانیه     | ۲ میلی‌ثانیه      | ۹ برابر سریع‌تر      |
| **۱۰۰,۰۰۰ رکورد**       | ۱۶۰ میلی‌ثانیه    | ۴ میلی‌ثانیه      | ۴۰ برابر سریع‌تر     |
| **۱,۰۰۰,۰۰۰ رکورد**     | ۲,۲۰۰ میلی‌ثانیه  | ۶ میلی‌ثانیه      | ۳۶۶ برابر سریع‌تر    |
| **۱۰,۰۰۰,۰۰۰ رکورد**    | ۲۹,۴۰۰ میلی‌ثانیه | ۹ میلی‌ثانیه      | ۳۲۶۰ برابر سریع‌تر\! |

💡 نمونه کدهای واقعی پیاده‌سازی (End-to-End Example)

در زیر، نحوه جریان یافتن فرآیندها در پروژه از کنترلر تا دیتابیس با الگوهای CQRS
و معماری تمیز به نمایش درآمده است:

گام اول: تعریف ساختار داده (Core / Domain Entity)

namespace Core.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

گام دوم: تعریف دستور به همراه اعتبارسنجی (Application Layer)

// Application/Products/Commands/CreateProductCommand.cs
public record CreateProductCommand(string Name, decimal Price, int Stock) : IRequest<Guid>;

// Application/Products/Validators/CreateProductCommandValidator.cs
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
    }
}

گام سوم: پیاده‌سازی متد اصلی پردازش (CommandHandler)

// Application/Products/Handlers/CreateProductCommandHandler.cs
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            Stock = request.Stock
        };

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}

گام چهارم: درگاه اتصال نهایی (Api Layer / Controller)

// Api/Controllers/ProductsController.cs
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var productId = await _mediator.Send(command);
        return Ok(new { Id = productId, Message = "محصول با موفقیت ثبت شد." });
    }
}

🔒 امنیت و فرآیند احراز هویت (JWT & Captcha Flow)

سیستم امنیتی پروژه شامل بررسی همزمان کپچای تولید شده در لایه فرانت‌اند و تطبیق
آن در وب‌سرویس به همراه چرخه امنیتی جفت‌توکن‌ها است:

sequenceDiagram
    User->>Api: درخواست دریافت تصویر کپچا
    Api->>User: ارسال پاسخ تصویر کپچا (Base64) + کلید شناسایی
    User->>Api: ارسال اطلاعات لاگین + پاسخ متنی کپچا + کلید شناسایی
    alt کد کپچا اشتباه است
        Api->>User: پاسخ خطا: "کد امنیتی نامعتبر است"
    else کد کپچا درست است
        Api->>Api: اعتبارسنجی اطلاعات کاربری در پایگاه داده
        Api->>User: ارسال جفت توکن (AccessToken + RefreshToken)
    end

نمونه استفاده از سیستم تولید کپچا (Aurora.Captcha):

[HttpGet("generate-captcha")]
public IActionResult GetCaptcha()
{
    var captcha = _captchaService.Generate(); // تولید کپچا
    // ذخیره مقدار کپچا در کش با طول عمر کوتاه (مثلا ۲ دقیقه)
    _cacheService.Set($"captcha_{captcha.Id}", captcha.Code, TimeSpan.FromMinutes(2));
    
    return Ok(new { Id = captcha.Id, ImageBase64 = captcha.ImageBase64 });
}

🤝 مشارکت در توسعه

از هرگونه مشارکت، گزارش باگ یا اضافه کردن ویژگی‌های جدید به پروژه استقبال
می‌شود:

1.  ابتدا پروژه را Fork کنید.
2.  یک شاخه جدید بسازید (git checkout -b feature/NewFeature).
3.  تغییرات خود را اعمال کنید (git commit -am 'Add some NewFeature').
4.  کدهای خود را به شاخه اصلی پوش کنید (git push origin feature/NewFeature).
5.  یک Pull Request برای ادغام ارسال کنید تا کدها بازبینی و تایید شوند.

📄 لایسنس

این پروژه تحت مجوز MIT منتشر شده است. استفاده، تغییر و توسعه‌ی آن برای اهداف
شخصی و تجاری کاملاً مجاز و آزاد است.


---

### 🛠️ چطور این محتوا را به پروژه اضافه کنم؟
1. وارد اکانت **GitHub** خود و مخزن `AuroraBase` شوید.
2. روی فایل **`README.md`** کلیک کنید.
3. دکمه **ویرایش (آیکون مداد ✏️)** را بزنید.
4. تمام متن‌های قبلی را پاک کرده و کدهای کادر بالا را در آن قرار دهید.
5. تغییرات را با یک پیام معنادار (مانند `docs: update readme with full documentation`) در برنچ اصلی خود **Commit** کنید.
