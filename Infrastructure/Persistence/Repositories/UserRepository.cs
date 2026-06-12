using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Application.Common.Models.Pagination;
using Application.Features.RoleFeatures.RoleManagement.Models;
using Application.Features.UsersFeature.ClaimManagement.Models;
using Application.Features.UsersFeature.UserManagement.Models;
using Core.Entities.Auth;
using Infrastructure.Persistence.Helpers;
using Infrastructure.Persistence.Repositories.Base;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Scrutor;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(MyContext context)
    : Repository<User, long>(context), IUserRepository
{
    public async Task<User> GetByUserNameAsync(string username, CancellationToken ct = default)
        => await dbSet.FirstOrDefaultAsync(u => u.UserName == username, ct);

    public async Task<User> GetByIdForAuthAsync(long id, CancellationToken ct = default)
        => await dbSet.FirstOrDefaultAsync(u => u.Id == id, ct);

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

    public async Task<PaginatedList<UserDto>> Search(UserIm search, CancellationToken ct)
    {
        var query = context.Users.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(search.FName))
            query = query.Where(x => x.FName.Contains(search.FName));

        if (!string.IsNullOrEmpty(search.LName))
            query = query.Where(x => x.LName.Contains(search.LName));

        if (!string.IsNullOrEmpty(search.PhoneNumber))
            query = query.Where(x => x.PhoneNumber.Contains(search.PhoneNumber));

        if (search.Status is not null)
            query = query.Where(x => x.Status == search.Status);

        return await query
            .ProjectToType<UserDto>()
            .ApplyPageBasedPaginationAsync(search, ct);
    }



    public async Task<IEnumerable<RoleDto>> GetUserRoles(long id, CancellationToken ct)
        => await context.UserRoles
            .Where(x => x.UserId == id)
            .Join(context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Title = r.Title
            }).ToListAsync(ct);

    public async Task<IEnumerable<BaseDropDown<int>>> UserRolesDropDown(long userId, string search, CancellationToken ct)
    {
        var roleQuery = context.Roles.AsNoTracking();

        if (!string.IsNullOrEmpty(search))
            roleQuery = roleQuery.Where(x => x.Title.Contains(search));

        var query = from r in roleQuery
                    join ur in context.UserRoles.Where(x => x.UserId == userId)
                        on r.Id equals ur.RoleId into joined
                    from subUr in joined.DefaultIfEmpty()
                    select new BaseDropDown<int>
                    {
                        Id = r.Id,
                        Value = r.Title,
                        IsSelected = subUr != null
                    };

        return await query.ToListAsync(ct);
    }

    #region user claims

    public void AddClaim(UserClaim claim)
        => context.UserClaims.Add(claim);

    public void UpdateClaim(UserClaim claim)
        => context.UserClaims.Update(claim);

    public void DeleteClaim(UserClaim claim)
        => context.UserClaims.Remove(claim);

    public async Task<IEnumerable<UserClaimDto>> GetClaims(long userId, CancellationToken cancellationToken)
        => await context.UserClaims.AsNoTracking()
        .Where(x => x.UserId == userId)
        .Select(x => new UserClaimDto
        {
            Id = x.Id,
            Type = x.ClaimType,
            Value = x.ClaimValue
        })
        .ToListAsync(cancellationToken);

    public async Task<UserClaim> GetClaimById(int id, CancellationToken cancellationToken)
        => await context.UserClaims.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    #endregion
}