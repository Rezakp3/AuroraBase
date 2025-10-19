using System.Reflection;
using Core.Entities;
using Core.Entities.Auth;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EfConfigs;

public class DBContext : DbContext
{
    #region Auth

    public DbSet<User> Users { get; set; }
    public DbSet<PasswordLogin> PasswordLogins { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RoleClaim> RoleClaims { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<UserClaim> UserClaims { get; set; }
    public DbSet<Service> Services { get; set; }

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

        var baseEntityType = typeof(BaseEntity<>);
        var baseEntityWithDateType = typeof(BaseEntityWithDate<>);

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

                // اگر مدل از BaseEntityWithDate ارث‌بری کرده باشد
                var baseType = type.BaseType;
                if (baseType != null && baseType.IsGenericType &&
                    baseType.GetGenericTypeDefinition() == baseEntityWithDateType)
                {
                    // مقدار پیش‌فرض CreatedDate برابر با GETDATE()
                    modelBuilder.Entity(type)
                        .Property("CreatedDate")
                        .HasDefaultValueSql("GETDATE()");
                }
            }

        #endregion

        var currentAssembly = Assembly.GetExecutingAssembly();
        modelBuilder.ApplyConfigurationsFromAssembly(currentAssembly);
        
        base.OnModelCreating(modelBuilder);
    }

    #endregion
}
