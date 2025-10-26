using Core.Entities.Auth.Relation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.AuthConfigs.RelationsConfig;

public class MenuServiceConfig : IEntityTypeConfiguration<MenuService>
{
    public void Configure(EntityTypeBuilder<MenuService> builder)
    {
        builder.ToTable("MenuService", "Auth");
        builder.HasKey(x => new { x.MenuId, x.ServiceId });

        builder.HasOne(x => x.Menu)
            .WithMany(x => x.MenuServices)
            .HasForeignKey(x => x.MenuId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Service)
            .WithMany(x => x.MenuServices)
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}