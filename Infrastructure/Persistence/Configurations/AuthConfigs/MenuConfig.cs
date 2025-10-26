using Core.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.AuthConfigs;

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

        builder.HasMany(x => x.RoleMenus)
            .WithOne(x => x.Menu)
            .HasForeignKey(x => x.MenuId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.MenuServices)
            .WithOne(x => x.Menu)
            .HasForeignKey(x => x.MenuId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
