using Application.Common.Interfaces.Generals;
using Application.Common.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence.Repositories;

public class UnitOfWork(MyContext context, IServiceProvider serviceProvider) : IUnitOfWork
{
    private IDbContextTransaction _transaction;

    public IUserRepository Users => GetService<IUserRepository>();
    public IRoleRepository Roles => GetService<IRoleRepository>();
    public IMenuRepository Menus => GetService<IMenuRepository>();
    public IServiceRepository Services => GetService<IServiceRepository>();
    public ISessionRepository Sessions => GetService<ISessionRepository>();
    public ISettingRepository Settings => GetService<ISettingRepository>();
    public IUserRoleRepository UserRoles => GetService<IUserRoleRepository>();
    public IRoleServiceRepository RoleServices => GetService<IRoleServiceRepository>();
    public IPasswordLoginRepository PasswordLogin => GetService<IPasswordLoginRepository>();


    #region navigations
    public bool SaveChanges()
    {
        var res = context.SaveChanges();
        return !(res <= 0);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var res = await context.SaveChangesAsync(cancellationToken);
        return !(res <= 0);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        => _transaction = await context.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            throw new InvalidOperationException("Transaction has not been started");

        try
        {
            await context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    private T GetService<T>() where T : notnull
        => serviceProvider.GetRequiredService<T>();

    #endregion

    public void Dispose()
    {
        _transaction?.Dispose();
        context.Dispose();
        GC.SuppressFinalize(this);
    }
}