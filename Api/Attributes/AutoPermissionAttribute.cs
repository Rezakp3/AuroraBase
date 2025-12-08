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
}
