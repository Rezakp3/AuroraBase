using Application.Common.Interfaces.Repositories;
using Core.Entities.Auth.Relation;
using Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Repositories;

public class RoleServiceRepository(MyContext context)
    : Repository<RoleService, int>(context), IRoleServiceRepository
{
    public async Task<HashSet<string>> GetPermittedIdentifiersAsync(int roleId, CancellationToken ct = default)
    {
        // یک کوئری Join بهینه برای گرفتن فقط رشته‌ها
        var identifiers = await context.RoleServices
            .AsNoTracking()
            .Where(rs => rs.RoleId == roleId)
            .Select(rs => rs.Service.ServiceIdentifier) // فیلدی که قبلا اضافه کردیم
            .ToListAsync(ct);

        return identifiers.ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public async Task SyncPermissionsAsync(int roleId, IEnumerable<int> serviceIds, CancellationToken ct = default)
    {
        // حذف قدیمی‌ها
        await dbSet.AsNoTracking().Where(rs => rs.RoleId == roleId).ExecuteDeleteAsync(ct);


        // افزودن جدیدها
        var newLinks = serviceIds.Select(sid => new RoleService { RoleId = roleId, ServiceId = sid });
        await dbSet.AddRangeAsync(newLinks, ct);
    }

    public async Task<List<int>> SyncRolesForServiceAsync(int serviceId, IEnumerable<int> roleIds, CancellationToken ct = default)
    {
        // ۱. پیدا کردن نقش‌هایی که در حال حاضر این سرویس را دارند (برای ابطال کش بعدی)
        var oldRoleIds = await dbSet
            .AsNoTracking()
            .Where(rs => rs.ServiceId == serviceId)
            .Select(rs => rs.RoleId)
            .ToListAsync(ct);

        // ۲. حذف تمام رابطه‌های فعلی این سرویس
        await dbSet.Where(rs => rs.ServiceId == serviceId).ExecuteDeleteAsync(ct);

        // ۳. افزودن رابطه‌های جدید
        var newLinks = roleIds.Select(rid => new RoleService { RoleId = rid, ServiceId = serviceId });
        await dbSet.AddRangeAsync(newLinks, ct);

        // ۴. تجمیع نقش‌های قدیمی و جدید (چون دسترسی هر دو گروه تغییر کرده است)
        var affectedRoleIds = oldRoleIds.Union(roleIds).ToList();

        return affectedRoleIds;
    }
}