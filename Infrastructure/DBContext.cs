using Core.Entities.Auth;
using Core.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Infrastructure;

public class DBContext : DbContext
{
    public DBContext() { }

    public DBContext(DbContextOptions options)
        : base(options) { }

    #region Db Sets

    #region Auth

    public virtual DbSet<Auth> Auth { get; set; }
    public virtual DbSet<AuthOtp> AuthOtp { get; set; }
    public virtual DbSet<AuthPassword> AuthPassword { get; set; }
    public virtual DbSet<Business> Businesses { get; set; }
    public virtual DbSet<GroupClaim> GroupClaims { get; set; }
    public virtual DbSet<RoleClaim> RoleClaims { get; set; }
    public virtual DbSet<City> Cities { get; set; }
    public virtual DbSet<Claim> Claims { get; set; }
    public virtual DbSet<Device> Devices { get; set; }
    public virtual DbSet<Group> Groups { get; set; }
    public virtual DbSet<Menu> Menus { get; set; }
    public virtual DbSet<MenuService> MenuServices { get; set; }
    public virtual DbSet<Profile> Profiles { get; set; }
    public virtual DbSet<Province> Provinces { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<RoleService> RoleServices { get; set; }
    public virtual DbSet<Service> Services { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserClaim> UserClaims { get; set; }
    public virtual DbSet<UserGroup> UserGroups { get; set; }
    public virtual DbSet<UserRole> UserRoles { get; set; }

    #endregion

    #endregion
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(DBContext).Assembly);
    }

    #region save change overrides

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
        => ChangeTracker.HasChanges() ? base.SaveChanges(acceptAllChangesOnSuccess) : 2000;

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        => ChangeTracker.HasChanges() ? base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken) : Task.FromResult(2000);

    #endregion
}
