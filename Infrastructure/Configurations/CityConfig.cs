using Core.Entities.Auth;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configurations;

public class CityConfig : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Code).HasMaxLength(20);

        builder.HasOne(x => x.Province)
            .WithMany(x => x.Cities)
            .HasForeignKey(x => x.ProvinceId);
    }
}
