using Application.Common.Interfaces.Generals;
using Core.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Interfaces.Repositories;

public interface IPasswordLoginRepository : IRepository<PasswordLogin, int>
{
    Task<PasswordLogin> GetByUserId(long userId, CancellationToken ct);
    Task<PasswordLogin> GetByEmail(string email, CancellationToken ct);
    Task<PasswordLogin> GetByResetToken(string resetPasswordToken, CancellationToken cancellationToken);
}
