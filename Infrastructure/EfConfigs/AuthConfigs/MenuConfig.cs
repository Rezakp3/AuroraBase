using Core.Entities.Auth;
using Core.Entities.Auth.Relation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EfConfigs.AuthConfigs;

public class MenuConfig : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.ToTable("Menu", "Auth");
        builder.Property(x => x.Title).HasMaxLength(50);

        builder.HasMany(x => x.SubMenu)
            .WithOne(x => x.Parent)
            .IsRequired(false)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Roles)
            .WithMany(x => x.Menus)
            .UsingEntity<RoleMenu>(
                r => r.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId),
                l => l.HasOne(x => x.Menu).WithMany().HasForeignKey(x => x.MenuId),
                j =>
                {
                    j.HasKey(x => new { x.RoleId, x.MenuId });
                    j.ToTable("RoleMenu", "Auth");
                });
    }
}
