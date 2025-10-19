using Core.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Entities.Auth.Relation;
using Core.Enums;

namespace Infrastructure.EfConfigs.AuthConfigs;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "Auth");
        builder.Property(x => x.Status).HasDefaultValue(EUserStatus.InActive);
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasMany(x => x.Roles)
            .WithMany(x => x.Users)
            .UsingEntity<UserRole>(
                l => l.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId),
                r => r.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId),
                j =>
                {
                    j.HasKey(x => new { x.RoleId, x.UserId });
                    j.ToTable("UserRole", "Auth");
                });

        builder.HasMany(x => x.Claims)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.RefreshTokens)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.PasswordLogin)
            .WithOne(x => x.User)
            .HasForeignKey<PasswordLogin>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
