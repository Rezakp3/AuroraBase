using Core.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.AuthConfigs;

internal class MenuConfig : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.ToTable("Menu", "Auth");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn().ValueGeneratedOnAdd();

        builder.Property(x => x.Title).HasMaxLength(50);
        builder.Property(x=>x.IsActive).HasDefaultValue(true);

        builder.Property(x => x.Route).HasMaxLength(150);
        builder.Property(x => x.Icon).HasMaxLength(30);

        builder.HasMany(x => x.SubMenu)
            .WithOne(x => x.Parent)
            .IsRequired(false)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.RoleMenus)
            .WithOne(x => x.Menu)
            .HasForeignKey(x => x.MenuId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.MenuServices)
            .WithOne(x => x.Menu)
            .HasForeignKey(x => x.MenuId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
