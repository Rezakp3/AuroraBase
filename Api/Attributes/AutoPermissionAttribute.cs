using Microsoft.AspNetCore.Authorization;

namespace Api.Attributes;

public class AutoPermissionAttribute : AuthorizeAttribute
{
    public const string PolicyName = "DynamicPermission";

    public AutoPermissionAttribute(bool allRoles = false)
    {
        AuthenticationSchemes = "Bearer";
        if (!allRoles)
            Policy = PolicyName;
    }
    public override bool Match(object? obj)
    {
        return base.Match(obj);
    }
}
