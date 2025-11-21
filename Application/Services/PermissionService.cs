using Application.Common.Interfaces.Generals;
using Application.Common.Interfaces.Services;

namespace Application.Services;

public class PermissionService(IUnitOfWork uow
    ) : IPermissionService
{
    private const string SecurityGroup = "Security";
    private const string SuperAdminsKey = "SuperAdminIds";

    public async Task<bool> HasPermissionAsync(long userId, string permissionIdentifier, CancellationToken ct = default)
    {
        // 1. دریافت لیست ID نقش‌های کاربر (از کش L1/L2)
        // کلید کش: auth:user:{userId}:roles
        var roleIds = await uow.UserRoles.GetUserRoleIdsAsync(userId, ct);

        if (roleIds == null || roleIds.Count == 0)
            return false;

        //2. بررسی اگر کاربر سوپر ادمین است
        var ids = await uow.Settings.GetValueAsync<List<long>>(SuperAdminsKey, SecurityGroup, ct);

        var isSuperAdmin = (from i in ids
                            join r in roleIds on i equals r
                            select r).Any();
                           
        if (isSuperAdmin)
            return true;

        var tasks = new List<Task<HashSet<string>>>(roleIds.Count);
        foreach (var roleId in roleIds)
        {
            // کلید کش: auth:role:{roleId}:perms
            tasks.Add(uow.RoleServices.GetPermittedIdentifiersAsync(roleId, ct));
        }

        // صبر می‌کنیم تا لیست پرمیشن‌های همه نقش‌ها بیاید
        var allRolesPermissions = await Task.WhenAll(tasks);

        // 4. جستجو
        foreach (var rolePerms in allRolesPermissions)
        {
            // HashSet.Contains عملیات O(1) است
            if (rolePerms != null && rolePerms.Contains(permissionIdentifier))
            {
                return true;
            }
        }

        return false;
    }

    public async Task<HashSet<string>> GetUserPermissionsAsync(long userId, CancellationToken ct = default)
    {
        var roleIds = await uow.UserRoles.GetUserRoleIdsAsync(userId, ct);

        if (roleIds == null || roleIds.Count == 0)
            return [];

        var tasks = roleIds.Select(id => uow.RoleServices.GetPermittedIdentifiersAsync(id, ct));
        var results = await Task.WhenAll(tasks);

        // تجمیع همه پرمیشن‌ها در یک HashSet واحد (حذف تکراری‌ها خودکار انجام می‌شود)
        var aggregatedPermissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var result in results)
        {
            if (result != null)
            {
                aggregatedPermissions.UnionWith(result);
            }
        }

        return aggregatedPermissions;
    }
}