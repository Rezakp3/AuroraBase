using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Core.Entities.Auth;
using Core.Entities.Relations;

namespace Infrastructure.Configurations;

public class GroupConfig : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(20);
        builder.Property(x => x.Description).HasMaxLength(100);

        builder
            .HasOne(x => x.Business)
            .WithMany(x => x.Groups)
            .HasForeignKey(x => x.BusinessKey);


        builder
            .HasMany(x => x.Claims)
            .WithMany(x => x.Groups)
            .UsingEntity<GroupClaim>(
                x => x.HasOne(x => x.Claim)
                    .WithMany(x => x.GroupClaims)
                    .HasForeignKey(x => x.ClaimId),
                x => x.HasOne(x => x.Group)
                    .WithMany(x => x.GroupClaims)
                    .HasForeignKey(x => x.GroupId).OnDelete(DeleteBehavior.NoAction)
            );
    }
}
