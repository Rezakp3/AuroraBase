using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Core.Entities.Auth;
using Core.Entities.Relations;

namespace Infrastructure.Configurations;

public class MenuConfig : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(50);
        builder.Property(x => x.Pname).HasMaxLength(50);
        builder.Property(x => x.Url).HasMaxLength(100);
        builder.Property(x => x.Icon).HasMaxLength(50);

        builder
            .HasOne(x => x.Business)
            .WithMany(x => x.Menues)
            .HasForeignKey(x => x.BusinessKey);

        builder
            .HasOne(x => x.Parent)
            .WithMany(x => x.Childs)
            .HasForeignKey(x => x.ParentId);

        builder
            .HasMany(x => x.Services)
            .WithMany(x => x.Menues)
            .UsingEntity<MenuService>(
                x => x.HasOne(x => x.Service)
                    .WithMany(x => x.MenuServices)
                    .HasForeignKey(x => x.ServiceId),
                x => x.HasOne(x => x.Menu)
                    .WithMany(x => x.MenuServices)
                    .HasForeignKey(x => x.MenuId)
            );
    }
}
