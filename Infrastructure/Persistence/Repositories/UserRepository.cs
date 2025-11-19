using Application.Common.Interfaces.Repositories;
using Core.Entities.Auth;
using Core.Enums;
using Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

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
}