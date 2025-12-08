using Core.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.AuthConfigs;

public class PasswordLoginConfig : IEntityTypeConfiguration<PasswordLogin>
{
    public void Configure(EntityTypeBuilder<PasswordLogin> builder)
    {
        builder.ToTable("PasswordLogin", "Auth");
        builder.HasKey(x => x.UserId);
        builder.Property(x => x.Email).HasMaxLength(30).IsRequired();
        builder.Property(x => x.EmailIsVerified).HasDefaultValue(false).ValueGeneratedOnAdd();
        builder.Property(x => x.EmailVerificationCode).HasMaxLength(50).IsRequired(false);
        builder.Property(x => x.VerifyCodeExpireDate).IsRequired(false);
        builder.Property(x => x.UserName).HasMaxLength(20).IsRequired();
        builder.Property(x => x.ResetPasswordToken).IsRequired(false);
        
        builder.HasOne(x => x.User)
            .WithOne(y => y.PasswordLogin)
            .HasForeignKey<PasswordLogin>(z => z.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
