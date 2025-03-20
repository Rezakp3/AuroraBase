using Core.Entities.Auth;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configurations;

public class ClaimConfig : IEntityTypeConfiguration<Claim>
{
    public void Configure(EntityTypeBuilder<Claim> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(20);
        builder.Property(x => x.Type).HasMaxLength(10);
        builder.HasOne(x => x.Business).WithMany(x => x.Claims).HasForeignKey(x => x.BusinessKey);
    }
}
