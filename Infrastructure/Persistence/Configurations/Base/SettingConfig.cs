using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Base;

public class SettingConfig : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.ToTable("Setting", "Base");
        builder.HasNoKey();

        builder.Property(x => x.Key).HasMaxLength(30);
        builder.Property(x => x.Value).HasMaxLength(100);
    }
}
