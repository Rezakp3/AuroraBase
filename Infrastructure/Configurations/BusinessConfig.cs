using Core.Entities.Auth;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configurations;

public class BusinessConfig : IEntityTypeConfiguration<Business>
{
    public void Configure(EntityTypeBuilder<Business> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(20);
        builder.Property(x => x.IsActive).HasDefaultValue(true);
        builder.Property(x => x.CreateDate);
        builder.Property(x => x.IsAdmin).HasDefaultValue(false);
        builder.Property(x => x.PublicKey).HasMaxLength(100);
        builder.Property(x => x.PrivateKey).HasMaxLength(100);
    }
}
