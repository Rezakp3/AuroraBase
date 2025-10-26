using Core.Entities.Auth.Relation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.AuthConfigs.RelationsConfig;

public class RoleMenuConfig : IEntityTypeConfiguration<RoleMenu>
{
    public void Configure(EntityTypeBuilder<RoleMenu> builder)
    {
        builder.ToTable("RoleMenu", "Auth");
        builder.HasKey(x => new { x.RoleId, x.MenuId });

        builder.HasOne(x => x.Role)
            .WithMany(x => x.RoleMenus)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Menu)
            .WithMany(x => x.RoleMenus)
            .HasForeignKey(x => x.MenuId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}