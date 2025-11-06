using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.CampusInfo;

namespace Rise.Persistence.Configurations.CampusInfo;

/// <summary>
/// Specific configuration for <see cref="Domain.CampusInfo.CampusInfo"/>.
/// </summary>
internal class CampusInfoConfiguration : EntityConfiguration<Domain.CampusInfo.CampusInfo>
{
    public override void Configure(EntityTypeBuilder<Domain.CampusInfo.CampusInfo> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Location)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.ContactPhone)
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(2000);

        // Configure Faculties as JSON column
        builder.Property(x => x.Faculties)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("json");
    }
}
