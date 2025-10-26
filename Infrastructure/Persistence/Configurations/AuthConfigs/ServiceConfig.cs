using Core.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.AuthConfigs;

public class ServiceConfig : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Service", "Auth");
        builder.Property(x => x.ServiceName).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(250).IsRequired(false);
        builder.Property(x => x.Address).HasMaxLength(60).IsRequired();

        builder.HasMany(x => x.MenuServices)
            .WithOne(x => x.Service)
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.RoleServices)
            .WithOne(x => x.Service)
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
