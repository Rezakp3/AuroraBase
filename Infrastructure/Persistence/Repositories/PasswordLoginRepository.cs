using Application.Common.Interfaces.Repositories;
using Core.Entities.Auth;
using Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class PasswordLoginRepository(MyContext context) 
    : Repository<PasswordLogin, int>(context), IPasswordLoginRepository
{
    public async Task<PasswordLogin> GetByEmail(string email, CancellationToken ct) 
        => await dbSet.FirstOrDefaultAsync(x => x.Email == email, ct);

    public async Task<PasswordLogin> GetByResetToken(string resetPasswordToken, CancellationToken cancellationToken)
        => await dbSet.FirstOrDefaultAsync(x => x.ResetPasswordToken == resetPasswordToken, cancellationToken);

    public async Task<PasswordLogin> GetByUserId(long userId, CancellationToken ct)
        => await dbSet.FirstOrDefaultAsync(x => x.UserId == userId, ct);
}
