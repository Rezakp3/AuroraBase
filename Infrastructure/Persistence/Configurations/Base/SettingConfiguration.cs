using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Base;

public class SettingConfiguration : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.ToTable("Settings", "Config");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Key)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Value)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.Group)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("General");

        builder.Property(x => x.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.DataType)
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue("String");

        builder.Property(x => x.IsEncrypted)
            .HasDefaultValue(false);

        // Composite unique index on Group + Key
        builder.HasIndex(x => new { x.Group, x.Key })
            .IsUnique()
            .HasDatabaseName("IX_Settings_Group_Key");

        // Index for fast group lookup
        builder.HasIndex(x => x.Group)
            .HasDatabaseName("IX_Settings_Group");
    }
}