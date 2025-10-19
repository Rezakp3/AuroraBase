using Core.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Entities.Auth.Relation;

namespace Infrastructure.EfConfigs.AuthConfigs;

public class RoleConfig : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Role", "Auth");
        builder.Property(x => x.Name).HasMaxLength(20);
        builder.Property(x => x.Title).IsRequired(false).HasMaxLength(50);


        builder
            .HasMany(x => x.Menus)
            .WithMany(x => x.Roles)
            .UsingEntity<RoleMenu>(
                l => l.HasOne(x => x.Menu).WithMany().HasForeignKey(x => x.MenuId),
                r => r.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId),
                j =>
                {
                    j.HasKey(x => new { x.RoleId, x.MenuId });
                    j.ToTable("RoleMenu", "Auth");
                });

        builder
            .HasMany(x => x.Services)
            .WithMany(x => x.Roles)
            .UsingEntity<RoleService>(
                l => l.HasOne(x => x.Service).WithMany().HasForeignKey(x => x.ServiceId),
                r => r.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId),
                j =>
                {
                    j.HasKey(x => new { x.ServiceId, x.RoleId });
                    j.ToTable("RoleService", "Auth");
                });

        builder
            .HasMany(x => x.Users)
            .WithMany(x => x.Roles)
            .UsingEntity<UserRole>(
                l => l.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId),
                r => r.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId),
                j =>
                {
                    j.HasKey(x => new { x.RoleId, x.UserId });
                    j.ToTable("UserRole", "Auth");
                }
            );

        builder.HasMany(x => x.Claims)
            .WithOne(x => x.Role)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
