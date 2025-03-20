using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Core.Entities.Auth;

namespace Infrastructure.Configurations;

public class ProvinceConfig : IEntityTypeConfiguration<Province>
{
    public void Configure(EntityTypeBuilder<Province> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(20);
        builder.Property(x => x.Code).HasMaxLength(10);
    }
}
