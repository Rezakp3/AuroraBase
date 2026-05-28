using Core.Entities.Auth;
using Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.AuthConfigs;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "Auth");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn().ValueGeneratedOnAdd();
        builder.Property(x => x.CreatedDate).HasDefaultValueSql("GetDate()");

        builder.Property(x => x.Status).HasDefaultValue(EUserStatus.InActive);
        builder.Property(x => x.FName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.LName).HasMaxLength(100);

        builder.Property(x => x.PhoneNumber).HasMaxLength(13);
        builder.Property(x => x.OtpCode).HasMaxLength(6);
        builder.Property(x => x.OtpExpireDate).IsRequired(false);

        builder.Property(x => x.UserName).HasMaxLength(50);
        builder.Property(x => x.LastUpdateDate).IsRequired(false);
        builder.Property(x => x.ResetPasswordToken).HasMaxLength(100);
        builder.Property(x => x.ResetPasswordTokenExpireDate).IsRequired(false);

        builder.HasMany(x => x.UserRoles)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Claims)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Sessions)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
