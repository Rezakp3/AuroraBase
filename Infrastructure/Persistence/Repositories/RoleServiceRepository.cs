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
        var existing = await dbSet.Where(rs => rs.RoleId == roleId).ToListAsync(ct);
        dbSet.RemoveRange(existing);

        // افزودن جدیدها
        var newLinks = serviceIds.Select(sid => new RoleService { RoleId = roleId, ServiceId = sid });
        await dbSet.AddRangeAsync(newLinks, ct);
    }
}