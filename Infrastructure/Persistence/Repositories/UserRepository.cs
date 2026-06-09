using Application.Common.Interfaces.Repositories;
using Application.Features.MenuFeature.Models;
using Application.Features.RoleFeatures.RoleManagement.Models;
using Application.Features.ServiceFeatures.Models;
using Application.Services.Models;
using Core.Entities.Auth;
using Core.Enums;
using DNTPersianUtils.Core;
using Infrastructure.Persistence.Repositories.Base;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Utils.Helpers;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(MyContext context) : Repository<User, long>(context), IUserRepository
{
    // این متد برای لاگین استفاده می‌شود
    public async Task<User> GetByUserNameAsync(string username, CancellationToken ct = default)
        => await dbSet.FirstOrDefaultAsync(u => u.UserName == username, ct);

    // این متد برای پروسه رفرش توکن و بررسی وضعیت کاربر استفاده می‌شود
    public async Task<User> GetByIdForAuthAsync(long id, CancellationToken ct = default) => await dbSet.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User> GetByPhoneNumberOrUsernameAsync(string key, CancellationToken ct = default) => await dbSet
            .FirstOrDefaultAsync(x => x.UserName == key || x.PhoneNumber == key, ct);

    public async Task<bool> UserNameExistForAdd(string username, CancellationToken ct = default)
        => await dbSet.AnyAsync(x => x.UserName.Trim().ToLower() == username, ct);

    public async Task<bool> UserNameExistForUpdate(long id, string username, CancellationToken ct = default)
        => await dbSet.AnyAsync(x => x.Id != id && x.UserName.Trim().ToLower() == username, ct);

    public async Task<bool> PhoneNumberExistForAdd(string email, CancellationToken ct = default)
        => await dbSet.AnyAsync(x => x.PhoneNumber.Trim().ToLower() == email, ct);

    public async Task<bool> PhoneNumberExistForUpdate(long id, string email, CancellationToken ct = default)
        => await dbSet.AnyAsync(x => x.Id != id && x.PhoneNumber.Trim().ToLower() == email, ct);

    public async Task<User> GetByResetToken(string resetPasswordToken, CancellationToken ct)
        => await dbSet.FirstOrDefaultAsync(x => x.ResetPasswordToken == resetPasswordToken, ct);

    public async Task<IEnumerable<RoleDto>> GetUserRoles(long id, CancellationToken ct)
        => await context.UserRoles
            .Where(x => x.UserId == id)
            .Join(context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Title = r.Title
            }).ToListAsync(ct);
}