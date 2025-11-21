using Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Infrastructure.Security.Authorization;

public class DynamicPermissionRequirement : IAuthorizationRequirement;

public class DynamicPermissionHandler(
    IPermissionService _permissionService,
    IHttpContextAccessor _httpContextAccessor
    ) : AuthorizationHandler<DynamicPermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        DynamicPermissionRequirement requirement)
    {
        // 1. بررسی اینکه کاربر لاگین است یا نه
        var userIdString = context.User.FindFirst("UserId")?.Value; // یا هر Claim که UserId توشه
        if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
        {
            context.Fail();
            return;
        }

        // 2. بدست آوردن اطلاعات Route (Controller & Action)
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            context.Fail();
            return;
        }

        var routeData = httpContext.GetRouteData();
        var controllerName = routeData.Values["controller"]?.ToString();
        var actionName = routeData.Values["action"]?.ToString();

        if (string.IsNullOrEmpty(controllerName) || string.IsNullOrEmpty(actionName))
        {
            // اگر نتوانستیم نام کنترلر یا اکشن را پیدا کنیم
            context.Fail();
            return;
        }

        // 3. ساختن کلید دسترسی (مثلاً: Order.Create)
        // نکته: مطمئن شوید در دیتابیس هم با همین فرمت ذخیره کرده‌اید
        var permissionIdentifier = $"{controllerName}.{actionName}";

        // 4. چک کردن دسترسی
        var hasPermission = await _permissionService.HasPermissionAsync(userId, permissionIdentifier);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
        else
        {
            // اگر Fail را صدا نزنیم، پروسه ادامه پیدا می‌کند (شاید هندلر دیگری موفق شود)
            // اما اینجا چون شرط قطعی است، اگر دسترسی نداشت، تمام است.
            context.Fail();
        }
    }
}
