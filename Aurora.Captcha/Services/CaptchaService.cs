using Aurora.Cache.Helpers;
using Aurora.Cache.Services;
using Aurora.Captcha.Helpers;
using Aurora.Captcha.Models;
using Aurora.Logger.Services;

namespace Aurora.Captcha.Services;

/// <summary>
/// سرویس مدیریت کپچا با استفاده از Cache
/// </summary>
public class CaptchaService(ICacheService cacheService) : ICaptchaService
{

    /// <summary>
    /// تولید کپچای جدید
    /// </summary>
    public async Task<CaptchaResult> GenerateAsync(
        int length = 5,
        int expirationMinutes = 5,
        int maxAttempts = 3,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // تولید شناسه یکتا
            var captchaId = Guid.NewGuid().ToString("N");

            // تولید کد و تصویر
            var (code, imageBase64) = CaptchaHelper.Generate(length);

            // ساخت داده برای ذخیره در کش
            var captchaData = new CaptchaData
            {
                Code = code,
                CreatedAt = DateTime.UtcNow,
                RemainingAttempts = maxAttempts,
                IsUsed = false
            };

            // ذخیره در کش با کلید استاندارد
            var cacheKey = CacheKeyBuilder.ForCaptcha(captchaId);
            var expiration = TimeSpan.FromMinutes(expirationMinutes);

            await cacheService.SetAsync(cacheKey, captchaData, expiration, cancellationToken);

            //logger.LogInfo($"Captcha generated: {captchaId}, Expires in {expirationMinutes} minutes",
                //new { captchaId, expirationMinutes });

            return new CaptchaResult
            {
                CaptchaId = captchaId,
                ImageBase64 = imageBase64,
                ExpiresInSeconds = (int)expiration.TotalSeconds
            };
        }
        catch (Exception ex)
        {
            //logger.LogError("Error generating captcha", ex);
            throw;
        }
    }

    /// <summary>
    /// اعتبارسنجی کپچا
    /// </summary>
    public async Task<CaptchaValidationResult> ValidateAsync(
        string captchaId,
        string userInput,
        bool caseSensitive = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // بررسی ورودی
            if (string.IsNullOrWhiteSpace(captchaId))
                return CaptchaValidationResult.Failure("شناسه کپچا نامعتبر است");

            if (string.IsNullOrWhiteSpace(userInput))
                return CaptchaValidationResult.Failure("لطفاً کد کپچا را وارد کنید");

            // دریافت از کش
            var cacheKey = CacheKeyBuilder.ForCaptcha(captchaId);
            var captchaData = await cacheService.GetAsync<CaptchaData>(cacheKey, cancellationToken);

            // بررسی وجود
            if (captchaData == null)
            {
                //logger.LogWarning("Captcha not found or expired: {CaptchaId}", captchaId);
                return CaptchaValidationResult.Failure("کپچا منقضی شده یا نامعتبر است");
            }

            // بررسی استفاده قبلی
            if (captchaData.IsUsed)
            {
                //logger.LogWarning("Captcha already used: {CaptchaId}", captchaId);
                return CaptchaValidationResult.Failure("این کپچا قبلاً استفاده شده است");
            }

            // بررسی تعداد تلاش‌ها
            if (captchaData.RemainingAttempts <= 0)
            {
                //logger.LogWarning("Captcha max attempts reached: {CaptchaId}", captchaId);
                await RemoveAsync(captchaId, cancellationToken);
                return CaptchaValidationResult.Failure("تعداد تلاش‌های مجاز تمام شده است", 0);
            }

            // اعتبارسنجی کد
            var isValid = CaptchaHelper.ValidateCode(captchaData.Code, userInput, caseSensitive);

            if (isValid)
            {
                // علامت‌گذاری به عنوان استفاده شده
                captchaData.IsUsed = true;
                await cacheService.SetAsync(cacheKey, captchaData, TimeSpan.FromMinutes(1), cancellationToken);

                //logger.LogInfo("Captcha validated successfully: {CaptchaId}", captchaId);
                return CaptchaValidationResult.Success();
            }
            else
            {
                // کاهش تعداد تلاش‌ها
                captchaData.RemainingAttempts--;
                await cacheService.SetAsync(cacheKey, captchaData, TimeSpan.FromMinutes(5), cancellationToken);

                //logger.LogWarning($"Invalid captcha attempt: {captchaId}, Remaining: {captchaData.RemainingAttempts}");

                return CaptchaValidationResult.Failure(
                    "کد کپچا اشتباه است",
                    captchaData.RemainingAttempts);
            }
        }
        catch (Exception ex)
        {
            //logger.LogError("Error validating captcha: {CaptchaId}", captchaId, ex);
            throw;
        }
    }

    /// <summary>
    /// حذف کپچا
    /// </summary>
    public async Task RemoveAsync(string captchaId, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = CacheKeyBuilder.ForCaptcha(captchaId);
            await cacheService.RemoveAsync(cacheKey, cancellationToken);

            //logger.LogInfo("Captcha removed: {CaptchaId}", captchaId);
        }
        catch (Exception ex)
        {
            //logger.LogError("Error removing captcha: {CaptchaId}", captchaId, ex);
            throw;
        }
    }

    /// <summary>
    /// تولید مجدد کپچا
    /// </summary>
    public async Task<CaptchaResult> RegenerateAsync(
        string captchaId,
        int length = 5,
        int expirationMinutes = 5,
        int maxAttempts = 3,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // حذف کپچای قبلی
            await RemoveAsync(captchaId, cancellationToken);

            // تولید کد و تصویر جدید
            var (code, imageBase64) = CaptchaHelper.Generate(length);

            // ساخت داده جدید
            var captchaData = new CaptchaData
            {
                Code = code,
                CreatedAt = DateTime.UtcNow,
                RemainingAttempts = maxAttempts,
                IsUsed = false
            };

            // ذخیره با همان ID
            var cacheKey = CacheKeyBuilder.ForCaptcha(captchaId);
            var expiration = TimeSpan.FromMinutes(expirationMinutes);

            await cacheService.SetAsync(cacheKey, captchaData, expiration, cancellationToken);

            //logger.LogInfo("Captcha regenerated: {CaptchaId}", captchaId);

            return new CaptchaResult
            {
                CaptchaId = captchaId,
                ImageBase64 = imageBase64,
                ExpiresInSeconds = (int)expiration.TotalSeconds
            };
        }
        catch (Exception ex)
        {
            //logger.LogError("Error regenerating captcha: {CaptchaId}", captchaId, ex);
            throw;
        }
    }
}