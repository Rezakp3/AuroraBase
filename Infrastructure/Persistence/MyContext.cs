using Core.Entities;
using Core.Entities.Auth;
using Core.Entities.Auth.Relation;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Infrastructure.Persistence;

public class MyContext : DbContext
{
    public MyContext(DbContextOptions<MyContext> options)
        : base(options) { }
    public MyContext() { }

    #region Auth

    public DbSet<User> Users => Set<User>();
    public DbSet<PasswordLogin> PasswordLogins => Set<PasswordLogin>();
    public DbSet<Session> Sessions => Set<Session>();
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
public class MyContextFactory : IDesignTimeDbContextFactory<MyContext>
{
    public MyContext CreateDbContext(string[] args)
    {
        // 1. بارگذاری فایل تنظیمات (appsettings.json)
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
            // فرض می‌کنیم appsettings.json در پروژه Startup قرار دارد.
            // اگر Factory در پروژه Data است، ممکن است نیاز به تعیین مسیر دقیق‌تر باشد.
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .Build();

        // 2. دریافت Connection String
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");
        }

        // 3. پیکربندی Options Builder
        var builder = new DbContextOptionsBuilder<MyContext>();

        // **مهم:** تعیین Provider و Assembly برای Migrationها
        builder.UseSqlServer(
            connectionString,
            // باید مطمئن شوید که Factory می‌داند Migrationها کجا هستند
            b => b.MigrationsAssembly(typeof(MyContext).Assembly.FullName)
        );

        // توجه: نیازی نیست که Interceptor را اینجا اضافه کنید، زیرا ابزارها برای ساخت شمای پایگاه داده به آن نیاز ندارند.

        // 4. ایجاد و بازگشت Context
        return new MyContext(builder.Options);
    }
}
