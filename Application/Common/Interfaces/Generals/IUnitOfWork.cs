using Application.Common.Interfaces.Repositories;
using Application.Resources;

namespace Application.Common.Interfaces.Generals;

/// <summary>
/// Unit of Work Pattern for managing transactions and repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // ✅ Repository های مشخص به جای Generic
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    IMenuRepository Menus { get; }
    IServiceRepository Services { get; }
    ISessionRepository Sessions { get; }
    ISettingRepository Settings { get; }
    IUserRoleRepository UserRoles { get; }
    IRoleServiceRepository RoleServices { get; }
    IPasswordLoginRepository PasswordLogin { get; }

    // Transaction Management
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
    bool SaveChanges();
    
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

