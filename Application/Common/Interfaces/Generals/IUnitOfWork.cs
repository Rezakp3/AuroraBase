using Application.Common.Interfaces.Repositories;

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
    IRefreshTokenRepository RefreshTokens { get; }
    ISettingRepository Settings { get; }
    
    // Transaction Management
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
    
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}