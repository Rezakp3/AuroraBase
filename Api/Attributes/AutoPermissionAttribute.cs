using Microsoft.AspNetCore.Authorization;

namespace Api.Attributes;

public class AutoPermissionAttribute : AuthorizeAttribute
{
    public const string PolicyName = "DynamicPermission";

    public AutoPermissionAttribute()
    {
        AuthenticationSchemes = "Bearer";
        Policy = PolicyName;
    }
}
