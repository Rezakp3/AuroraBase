using Application.Features.Auth.Models;
using Core.Entities.Auth;

namespace Application.Common.Interfaces.Services;

/// <summary>
/// سرویس مشترک برای مدیریت توکن، نشست و بارگذاری داده‌های اولیه احراز هویت
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// تولید Access Token و Refresh Token جدید، ایجاد نشست در دیتابیس و بارگذاری منوها/نقش‌ها
    /// </summary>
    /// <param name="user">موجودیت کاربر</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>TokenVm حاوی توکن‌ها و داده‌های اولیه</returns>
    Task<TokenVm> GenerateAuthTokensAndSessionAsync(User user, CancellationToken cancellationToken);

    /// <summary>
    /// ابطال نشست قدیمی و تولید توکن‌های جدید (برای Refresh Token Rotation)
    /// </summary>
    Task<TokenVm> RotateTokensAndSessionAsync(User user, Session oldSession, CancellationToken cancellationToken);
}