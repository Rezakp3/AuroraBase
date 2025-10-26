using System.Reflection;
using Core.Entities;
using Core.Entities.Auth;
using Core.Entities.Auth.Relation;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class MyContext(DbContextOptions<MyContext> options) : DbContext(options)
{

    #region Auth

    public DbSet<User> Users => Set<User>();
    public DbSet<PasswordLogin> PasswordLogins => Set<PasswordLogin>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RoleClaim> RoleClaims => Set<RoleClaim>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<UserClaim> UserClaims => Set<UserClaim>();
    public DbSet<Service> Services => Set<Service>();

    // Junction Tables
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RoleMenu> RoleMenus => Set<RoleMenu>();
    public DbSet<RoleService> RoleServices => Set<RoleService>();
    public DbSet<MenuService> MenuServices => Set<MenuService>();

    #endregion

    #region Base

    public DbSet<Setting> Settings => Set<Setting>();

    #endregion

    #region save changes

    public override int SaveChanges()
        => SaveChanges(true);

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        if (ChangeTracker.HasChanges())
            return base.SaveChanges(acceptAllChangesOnSuccess);
        else
            return -1;
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => SaveChangesAsync(true, cancellationToken);

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        if (ChangeTracker.HasChanges())
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        else
            return Task.FromResult(-1);
    }

    #endregion

    #region OnModelCreating

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region base type configs

        var baseEntityType = typeof(IBaseEntity<>);

        var entityTypes = Assembly.GetAssembly(typeof(User))?
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.BaseType != null && t.BaseType.IsGenericType &&
                        t.BaseType.GetGenericTypeDefinition() == baseEntityType);

        if (entityTypes is not null)
            foreach (var type in entityTypes)
            {
                // تعریف کلید اصلی برای فیلد Id
                modelBuilder.Entity(type).HasKey("Id");
                modelBuilder.Entity(type)
                    .Property("Id")
                    .ValueGeneratedOnAdd()
                    .UseIdentityColumn();
            }

        #endregion

        var currentAssembly = Assembly.GetExecutingAssembly();
        modelBuilder.ApplyConfigurationsFromAssembly(currentAssembly);

        base.OnModelCreating(modelBuilder);
    }

    #endregion
}
