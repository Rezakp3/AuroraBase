using Core.Entities.Auth;
using Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations;

public class ProfileConfig : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.HasBaseType<User>().ToTable(nameof(Profile));
        builder.Property(x => x.Image).HasMaxLength(50);
        builder.Property(x => x.FirstName).HasMaxLength(50);
        builder.Property(x => x.LastName).HasMaxLength(50);
        builder.Ignore(x => x.FullName);

        builder
            .HasOne(x => x.CityNavigation)
            .WithMany(x => x.Profiles)
            .HasForeignKey(x => x.CityId);
    }
}
