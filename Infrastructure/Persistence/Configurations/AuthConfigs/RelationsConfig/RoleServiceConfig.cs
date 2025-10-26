using Core.Entities.Auth.Relation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.AuthConfigs.RelationsConfig;

public class RoleServiceConfig : IEntityTypeConfiguration<RoleService>
{
    public void Configure(EntityTypeBuilder<RoleService> builder)
    {
        builder.ToTable("RoleService", "Auth");
        builder.HasKey(x => new { x.RoleId, x.ServiceId });

        builder.HasOne(x => x.Role)
            .WithMany(x => x.RoleServices)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Service)
            .WithMany(x => x.RoleServices)
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}