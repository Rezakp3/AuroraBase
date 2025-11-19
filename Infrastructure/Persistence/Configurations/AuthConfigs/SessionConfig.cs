using Core.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.AuthConfigs;

public class SessionConfig : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("Session", "Auth");
        builder.Property(x => x.IsRevoked).HasDefaultValue(false).ValueGeneratedOnAdd();
        builder.Property(x => x.DeviceName).IsRequired(false);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Sessions)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
