using Core.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.AuthConfigs;

public class RoleClaimConfig : IEntityTypeConfiguration<RoleClaim>
{
    public void Configure(EntityTypeBuilder<RoleClaim> builder)
    {
        builder.ToTable("RefreshToken", "Auth");
        builder.Property(x => x.Type).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(30).IsRequired();

        builder
            .HasOne(x => x.Role)
            .WithMany(x => x.Claims)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
