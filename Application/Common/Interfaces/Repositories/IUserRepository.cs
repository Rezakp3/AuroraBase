using Application.Common.Interfaces.Generals;
using Application.Features.RoleFeatures.RoleManagement.Models;
using Application.Services.Models;
using Core.Entities.Auth;

namespace Application.Common.Interfaces.Repositories;

public interface IUserRepository : IRepository<User, long>
{
    Task<User> GetByUserNameAsync(string username, CancellationToken ct = default);
    Task<User> GetByIdForAuthAsync(long id, CancellationToken ct = default);
    Task<User> GetByPhoneNumberOrUsernameAsync(string key, CancellationToken ct = default);
    Task<bool> UserNameExistForAdd(string username, CancellationToken ct = default);
    Task<bool> UserNameExistForUpdate(long id, string username, CancellationToken ct = default);
    Task<bool> PhoneNumberExistForAdd(string phoneNumber, CancellationToken ct = default);
    Task<bool> PhoneNumberExistForUpdate(long id, string PhoneNumber, CancellationToken ct = default);
    Task<User> GetByResetToken(string resetPasswordToken, CancellationToken ct = default);
    Task<IEnumerable<RoleDto>> GetUserRoles(long id, CancellationToken ct);
}