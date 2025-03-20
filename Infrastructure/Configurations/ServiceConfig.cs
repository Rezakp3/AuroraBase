using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Core.Entities.Auth;

namespace Infrastructure.Configurations;

public class ServiceConfig : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Pname).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(100);
        builder.Property(x => x.MicroServiceName).HasMaxLength(20);

        builder.HasOne(x => x.Business).WithMany(x => x.Services).HasForeignKey(x => x.BusinessKey);
    }
}
