using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Common.Models.Pagination;
using Application.Features.RoleFeatures.RoleManagement.Models;
using Application.Features.UsersFeature.ClaimManagement.Models;
using Application.Features.UsersFeature.UserManagement.Models;
using Application.Services.Models;
using Core.Entities.Auth;
using Core.Enums;

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
    Task<PaginatedList<UserDto>> Search(UserIm search, CancellationToken ct);
    Task<IEnumerable<UserClaimDto>> GetClaims(long userId, CancellationToken cancellationToken);
    void AddClaim(UserClaim claim);
    void UpdateClaim(UserClaim claim);
    void DeleteClaim(UserClaim claim);
    Task<UserClaim> GetClaimById(int id, CancellationToken cancellationToken);
    Task<IEnumerable<BaseDropDown<int>>> UserRolesDropDown(long userId, string search, CancellationToken ct);
}