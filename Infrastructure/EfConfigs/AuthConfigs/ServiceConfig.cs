using Core.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Entities.Auth.Relation;

namespace Infrastructure.EfConfigs.AuthConfigs;

public class ServiceConfig : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("MenuService", "Auth");
        builder.Property(x => x.ServiceName).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(250).IsRequired(false);
        builder.Property(x => x.Address).HasMaxLength(60).IsRequired();

        builder.HasMany(x => x.Menus)
            .WithMany(x => x.Services)
            .UsingEntity<MenuService>(
                l => l.HasOne(x => x.Menu).WithMany().HasForeignKey(x => x.MenuId),
                r => r.HasOne(x => x.Service).WithMany().HasForeignKey(x => x.ServiceId),
                j =>
                {
                    j.HasKey(x => new { x.MenuId, x.ServiceId });
                    j.ToTable("MenuService", "Auth");
                }
            );

        builder.HasMany(x => x.Roles)
            .WithMany(x => x.Services)
            .UsingEntity<RoleService>(
                l => l.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId),
                r => r.HasOne(x => x.Service).WithMany().HasForeignKey(x => x.ServiceId),
                j => j.HasKey(x => new { x.RoleId, x.ServiceId })
            );
    }
}
