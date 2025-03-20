using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Core.Entities.Auth;
using Core.Entities.Relations;

namespace Infrastructure.Configurations;

public class RoleConfig : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Pname).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(100);

        builder.HasOne(x => x.Business).WithMany().HasForeignKey(x => x.BusinessKey);

        builder.HasMany(x => x.Claims)
            .WithMany(x => x.Roles)
            .UsingEntity<RoleClaim>(
                x => x.HasOne(x => x.Claim)
                    .WithMany(x => x.RoleClaims)
                    .HasForeignKey(x => x.ClaimId),
                x => x.HasOne(x => x.Role)
                    .WithMany(x => x.RoleClaims)
                    .HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.NoAction)
            );

        builder.HasMany(x => x.Services)
            .WithMany(x => x.Roles)
            .UsingEntity<RoleService>(
                x => x.HasOne(x => x.Service)
                    .WithMany(x => x.RoleServices)
                    .HasForeignKey(x => x.ServiceId),
                x => x.HasOne(x => x.Role)
                    .WithMany(x => x.RoleServices)
                    .HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.NoAction)
            );

    }
}