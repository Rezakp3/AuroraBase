using Core.Entities.Auth;
using Core.Entities.Relations;
using Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.PhoneNo).HasMaxLength(15).IsRequired();
        builder.HasIndex(x => new { x.PhoneNo, x.BusinessKey }).IsUnique(true);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.Email).HasMaxLength(100);
        builder.Property(x => x.CreateDate);
        builder.Property(x => x.Status).HasDefaultValue(EUserStatus.Active);
        builder.Property(x => x.Username).HasMaxLength(50);

        builder.HasOne(x => x.Auth).WithOne(x => x.User).HasForeignKey<Auth>(x => x.UserId);
        builder.HasOne(x => x.Parent).WithMany(x => x.Childs).HasForeignKey(x => x.ParentId);

        builder.HasMany(x => x.Claims)
            .WithMany(x => x.Users)
            .UsingEntity<UserClaim>(
                x => x.HasOne(x => x.Claim)
                    .WithMany(x => x.UserClaims)
                    .HasForeignKey(x => x.ClaimId),
                x => x.HasOne(x => x.User)
                    .WithMany(x => x.UserClaims)
                    .HasForeignKey(x => x.UserId)
            );

        builder.HasMany(x => x.Groups)
            .WithMany(x => x.Users)
            .UsingEntity<UserGroup>(
                x => x.HasOne(x => x.Group)
                    .WithMany(x => x.UserGroups)
                    .HasForeignKey(x => x.GroupId),
                x => x.HasOne(x => x.User)
                    .WithMany(x => x.UserGroups)
                    .HasForeignKey(x => x.UserId)
            );
        builder.HasMany(x => x.Roles)
            .WithMany(x => x.Users)
            .UsingEntity<UserRole>(
                x => x.HasOne(x => x.Role)
                    .WithMany(x => x.UserRoles)
                    .HasForeignKey(x => x.RoleId),
                x => x.HasOne(x => x.User)
                    .WithMany(x => x.UserRoles)
                    .HasForeignKey(x => x.UserId)
            );
    }
}
