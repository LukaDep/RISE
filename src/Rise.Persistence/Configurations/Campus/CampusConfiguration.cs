using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Rise.Persistence.Configurations.Campus;

/// <summary>
/// Specific configuration for <see cref="Domain.Campus.Campus"/>.
/// Configures property constraints and JSON serialization for facilities.
/// </summary>
internal class CampusConfiguration : EntityConfiguration<Domain.Campus.Campus>
{
    /// <summary>
    /// Configures the Campus entity properties including address fields and JSON facilities list.
    /// </summary>
    /// <param name="builder">The entity type builder for Campus.</param>
    public override void Configure(EntityTypeBuilder<Domain.Campus.Campus> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Street)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.HouseNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.PostalCode)
            .IsRequired()
            .HasMaxLength(20);


        builder.Property(x => x.ContactPhone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.Facilities)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => string.IsNullOrWhiteSpace(v)
                    ? new List<string>()
                    : JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("TEXT");

        // Configure one-to-many relationship with Building
        builder.HasMany(x => x.Buildings)
            .WithOne()
            .HasForeignKey("CampusId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
