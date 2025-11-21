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
    {
        return await dbSet
            .Include(u => u.PasswordLogin) // اینکلود کردن اطلاعات لاگین
            .FirstOrDefaultAsync(u => u.PasswordLogin.UserName == username, ct);
    }

    // این متد برای پروسه رفرش توکن و بررسی وضعیت کاربر استفاده می‌شود
    public async Task<User> GetByIdForAuthAsync(long id, CancellationToken ct = default)
    {
        return await dbSet
            .Include(u => u.PasswordLogin)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<User> GetByEmailOrUsernameAsync(string key, CancellationToken ct = default)
        => await dbSet.Include(x => x.PasswordLogin)
        .FirstOrDefaultAsync(x => x.PasswordLogin.UserName == key || x.PasswordLogin.Email == key, ct);

    public async Task<bool> UserNameOrEmailExistForAdd(string email, string username, CancellationToken ct = default)
        => await context.PasswordLogins
        .AnyAsync(x => x.UserName.ToLower() == username.Trim()
        || x.Email.Trim().ToLower() == email.Trim().ToLower(), ct);

    public async Task<bool> UserNameOrEmailExistForUpdate(long id, string email, string username, CancellationToken ct = default)
        => await context.PasswordLogins.AnyAsync(x => x.Id != id && (x.UserName.Eq(username) || x.Email.Eq(email)), ct);
}