using Core.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.AuthConfigs;

public class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshToken", "Auth");
        builder.Property(x => x.IsRevoked).HasDefaultValue(false).ValueGeneratedOnAdd();
        builder.Property(x => x.DeviceName).IsRequired(false);

        builder.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
