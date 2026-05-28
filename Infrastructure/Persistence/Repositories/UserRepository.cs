using Application.Common.Interfaces.Repositories;
using Core.Entities.Auth;
using Core.Enums;
using Infrastructure.Persistence.Repositories.Base;
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
}