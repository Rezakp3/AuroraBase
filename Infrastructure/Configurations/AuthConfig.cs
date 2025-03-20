using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Core.Entities.Auth;
using Core.Enums;

namespace Infrastructure.Configurations;

public class AuthConfig : IEntityTypeConfiguration<Auth>
{
    public void Configure(EntityTypeBuilder<Auth> builder)
    {
        builder
            .HasDiscriminator<AuthType>("Type")
            .HasValue<Auth>(AuthType.Base)
            .HasValue<AuthOtp>(AuthType.Otp)
            .HasValue<AuthPassword>(AuthType.Password)
            .IsComplete(false);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.CreateDate);
        builder.Property(x => x.PhoneNo).HasMaxLength(15);
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.RefreshToken).HasMaxLength(50);

        builder.HasOne(x => x.User).WithOne(x => x.Auth);
        builder.HasOne(x => x.Business).WithMany(x => x.Auths).HasForeignKey(x => x.BusinessKey);
    }
}
public class AuthOtpConfig : IEntityTypeConfiguration<AuthOtp>
{
    public void Configure(EntityTypeBuilder<AuthOtp> builder)
    {
        builder.HasBaseType<Auth>();
    }
}
public class AuthPasswordConfig : IEntityTypeConfiguration<AuthPassword>
{
    public void Configure(EntityTypeBuilder<AuthPassword> builder)
    {
        builder.HasBaseType<Auth>();
        builder.Property(x => x.Password).HasMaxLength(50);
        builder.Property(x => x.Salt).HasMaxLength(50);
    }
}
