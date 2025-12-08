using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Text;

namespace Aurora.Captcha.Helpers;

/// <summary>
/// کلاس کمکی برای تولید کپچا (Cross-Platform)
/// </summary>
public static class CaptchaHelper
{
    private static readonly Random Random = new();

    // کاراکترهای مجاز در کپچا (بدون کاراکترهای مشابه مثل O و 0، I و l)
    private static readonly char[] Characters = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789".ToCharArray();

    #region Generate Code

    /// <summary>
    /// تولید کد تصادفی کپچا
    /// </summary>
    /// <param name="length">طول کد (پیش‌فرض: 5)</param>
    /// <returns>کد تولید شده</returns>
    public static string GenerateCode(int length = 5)
    {
        if (length < 3 || length > 10)
            throw new ArgumentException("طول کد باید بین 3 تا 10 باشد", nameof(length));

        var code = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            code.Append(Characters[Random.Next(Characters.Length)]);
        }

        return code.ToString();
    }

    #endregion

    #region Generate Image

    /// <summary>
    /// تولید تصویر کپچا به صورت Base64
    /// </summary>
    /// <param name="code">کد کپچا (اگر null باشد، خودکار تولید می‌شود)</param>
    /// <param name="width">عرض تصویر (پیش‌فرض: 200)</param>
    /// <param name="height">ارتفاع تصویر (پیش‌فرض: 70)</param>
    /// <param name="noiseLines">تعداد خطوط نویز (پیش‌فرض: 5)</param>
    /// <returns>تصویر کپچا به صورت Base64</returns>
    public static string GenerateImageBase64(string? code = null, int width = 200, int height = 70, int noiseLines = 5)
    {
        code ??= GenerateCode();

        using var image = new Image<Rgba32>(width, height);

        // رسم کپچا
        DrawCaptcha(image, code, width, height, noiseLines);

        // تبدیل به Base64
        using var memoryStream = new MemoryStream();
        image.SaveAsPng(memoryStream);
        return Convert.ToBase64String(memoryStream.ToArray());
    }

    /// <summary>
    /// تولید تصویر کپچا به صورت Stream
    /// </summary>
    /// <param name="code">کد کپچا</param>
    /// <param name="width">عرض تصویر</param>
    /// <param name="height">ارتفاع تصویر</param>
    /// <param name="noiseLines">تعداد خطوط نویز</param>
    /// <returns>Stream تصویر</returns>
    public static MemoryStream GenerateImageStream(string? code = null, int width = 200, int height = 70, int noiseLines = 5)
    {
        code ??= GenerateCode();

        using var image = new Image<Rgba32>(width, height);

        // رسم کپچا
        DrawCaptcha(image, code, width, height, noiseLines);

        var memoryStream = new MemoryStream();
        image.SaveAsPng(memoryStream);
        memoryStream.Position = 0;

        return memoryStream;
    }

    /// <summary>
    /// تولید تصویر کپچا به صورت byte array
    /// </summary>
    /// <param name="code">کد کپچا</param>
    /// <param name="width">عرض تصویر</param>
    /// <param name="height">ارتفاع تصویر</param>
    /// <param name="noiseLines">تعداد خطوط نویز</param>
    /// <returns>آرایه بایت تصویر</returns>
    public static byte[] GenerateImageBytes(string? code = null, int width = 200, int height = 70, int noiseLines = 5)
    {
        code ??= GenerateCode();

        using var image = new Image<Rgba32>(width, height);

        // رسم کپچا
        DrawCaptcha(image, code, width, height, noiseLines);

        using var memoryStream = new MemoryStream();
        image.SaveAsPng(memoryStream);
        return memoryStream.ToArray();
    }

    #endregion

    #region Drawing Methods

    /// <summary>
    /// رسم کامل کپچا روی تصویر
    /// </summary>
    private static void DrawCaptcha(Image<Rgba32> image, string code, int width, int height, int noiseLines)
    {
        image.Mutate(ctx =>
        {
            // پس‌زمینه با گرادیانت
            DrawBackground(ctx, width, height);

            // افزودن نویز پیکسلی قبل از متن
            AddPixelNoise(ctx, width, height);

            // نوشتن متن کپچا
            DrawCaptchaText(ctx, code, width, height);

            // افزودن خطوط نویز
            DrawNoiseLines(ctx, width, height, noiseLines);
        });
    }

    /// <summary>
    /// رسم پس‌زمینه با گرادیانت
    /// </summary>
    private static void DrawBackground(IImageProcessingContext ctx, int width, int height)
    {
        // رنگ روشن برای پس‌زمینه
        var backgroundColor = GetRandomLightColor();
        ctx.Fill(backgroundColor);

        // افزودن گرادیانت ساده با شفافیت
        var gradientColor = GetRandomLightColor();
        var gradientBrush = gradientColor.WithAlpha(150);

        ctx.Fill(gradientBrush, new RectangleF(0, height / 2, width, height / 2));
    }

    /// <summary>
    /// افزودن نویز پیکسلی
    /// </summary>
    private static void AddPixelNoise(IImageProcessingContext ctx, int width, int height)
    {
        int noisePixels = (width * height) / 40; // حدود 2.5% پیکسل‌ها

        for (int i = 0; i < noisePixels; i++)
        {
            int x = Random.Next(width);
            int y = Random.Next(height);

            var color = GetRandomDarkColor();
            ctx.Fill(color, new RectangleF(x, y, 1, 1));
        }
    }

    /// <summary>
    /// نوشتن متن کپچا با رنگ‌های مختلف و موقعیت‌های تصادفی
    /// </summary>
    private static void DrawCaptchaText(IImageProcessingContext ctx, string code, int width, int height)
    {
        try
        {
            // انتخاب فونت
            var font = GetFont(height);
            if (font == null)
            {
                // اگر فونت در دسترس نبود، از Fallback استفاده کن
                DrawSimpleText(ctx, code, width, height);
                return;
            }

            // محاسبه اندازه متن
            var textOptions = new TextOptions(font);
            var textSize = TextMeasurer.MeasureSize(code, textOptions);

            // محاسبه موقعیت شروع (مرکز)
            float startX = (width - textSize.Width) / 3;
            float currentX = startX;
            float baseY = (height - textSize.Height) / 2;

            // نوشتن هر کاراکتر با رنگ و موقعیت مختلف
            foreach (char c in code)
            {
                var charText = c.ToString();
                var charSize = TextMeasurer.MeasureSize(charText, textOptions);

                // موقعیت Y با تغییر تصادفی
                float y = baseY + Random.Next(-10, 10);

                // رنگ تصادفی
                var textColor = GetRandomDarkColor();

                // نوشتن کاراکتر
                var drawingOptions = new RichTextOptions(font)
                {
                    Origin = new PointF(currentX, y)
                };

                ctx.DrawText(drawingOptions, charText, textColor);

                // فاصله بین کاراکترها (با تغییرات تصادفی)
                currentX += charSize.Width * Random.Next(100, 200) / 100f;
            }
        }
        catch (Exception)
        {
            // در صورت خطا، از متن ساده استفاده کن
            DrawSimpleText(ctx, code, width, height);
        }
    }

    /// <summary>
    /// انتخاب و ایجاد فونت مناسب
    /// </summary>
    private static Font? GetFont(int height)
    {
        try
        {
            // لیست فونت‌های پیشنهادی
            string[] fontNames = { "Arial", "Verdana", "Tahoma", "DejaVu Sans", "Liberation Sans", "Helvetica" };

            // جستجو برای یافتن فونت موجود
            FontFamily? fontFamily = null;

            foreach (var fontName in fontNames)
            {
                fontFamily = SystemFonts.Families.FirstOrDefault(f =>
                    f.Name.Equals(fontName, StringComparison.OrdinalIgnoreCase));

                if (fontFamily != null)
                    break;
            }

            // اگر هیچ فونت پیشنهادی پیدا نشد، از اولین فونت موجود استفاده کن
            fontFamily ??= SystemFonts.Families.FirstOrDefault();

            if (fontFamily == null)
                return null;

            float fontSize = height * 0.5f;
            return fontFamily.Value.CreateFont(fontSize, FontStyle.Bold);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// رسم متن ساده (Fallback)
    /// </summary>
    private static void DrawSimpleText(IImageProcessingContext ctx, string code, int width, int height)
    {
        try
        {
            if (!SystemFonts.Families.Any())
                return;
            
            var fontFamily = SystemFonts.Families.FirstOrDefault();

            var font = fontFamily.CreateFont(height * 0.4f, FontStyle.Bold);

            var drawingOptions = new RichTextOptions(font)
            {
                Origin = new PointF(width / 6, height / 3)
            };

            ctx.DrawText(drawingOptions, code, GetRandomDarkColor());
        }
        catch
        {
            // اگر حتی Fallback هم کار نکرد، چیزی رسم نمی‌شود
        }
    }

    /// <summary>
    /// رسم خطوط نویز
    /// </summary>
    private static void DrawNoiseLines(IImageProcessingContext ctx, int width, int height, int count)
    {
        for (int i = 0; i < count; i++)
        {
            var color = GetRandomDarkColor();
            var thickness = Random.Next(1, 3);

            // خط ساده
            if (Random.Next(2) == 0)
            {
                var startPoint = new PointF(Random.Next(width), Random.Next(height));
                var endPoint = new PointF(Random.Next(width), Random.Next(height));

                ctx.DrawLine(color, thickness, startPoint, endPoint);
            }
            // منحنی با چند نقطه
            else
            {
                var points = new PointF[]
                {
                    new(Random.Next(width), Random.Next(height)),
                    new(Random.Next(width), Random.Next(height)),
                    new(Random.Next(width), Random.Next(height))
                };

                ctx.DrawLine(color, thickness, points);
            }
        }
    }

    #endregion

    #region Color Helpers

    /// <summary>
    /// تولید رنگ روشن تصادفی (برای پس‌زمینه)
    /// </summary>
    private static Color GetRandomLightColor()
    {
        return Color.FromRgb(
            (byte)Random.Next(220, 255),
            (byte)Random.Next(220, 255),
            (byte)Random.Next(220, 255));
    }

    /// <summary>
    /// تولید رنگ تیره تصادفی (برای متن و خطوط)
    /// </summary>
    private static Color GetRandomDarkColor()
    {
        return Color.FromRgb(
            (byte)Random.Next(0, 120),
            (byte)Random.Next(0, 120),
            (byte)Random.Next(0, 120));
    }

    #endregion

    #region Validation Helpers

    /// <summary>
    /// مقایسه کد کپچا (بدون حساسیت به حروف بزرگ/کوچک)
    /// </summary>
    /// <param name="code">کد اصلی</param>
    /// <param name="userInput">ورودی کاربر</param>
    /// <param name="caseSensitive">حساس به حروف بزرگ/کوچک (پیش‌فرض: false)</param>
    /// <returns>true اگر مطابقت داشته باشد</returns>
    public static bool ValidateCode(string code, string userInput, bool caseSensitive = false)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(userInput))
            return false;

        return caseSensitive
            ? code.Equals(userInput.Trim())
            : code.Equals(userInput.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region Generate with Code (Helper)

    /// <summary>
    /// تولید کد و تصویر کپچا به صورت همزمان
    /// </summary>
    /// <param name="length">طول کد</param>
    /// <param name="width">عرض تصویر</param>
    /// <param name="height">ارتفاع تصویر</param>
    /// <param name="noiseLines">تعداد خطوط نویز</param>
    /// <returns>Tuple شامل کد و تصویر Base64</returns>
    public static (string Code, string ImageBase64) Generate(int length = 5, int width = 200, int height = 70, int noiseLines = 5)
    {
        string code = GenerateCode(length);
        string imageBase64 = GenerateImageBase64(code, width, height, noiseLines);

        return (code, imageBase64);
    }

    #endregion
}