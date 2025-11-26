using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Restos;

namespace Rise.Persistence.Configurations.Restos;

internal class RestoConfiguration : EntityConfiguration<Resto>
{

    public override void Configure(EntityTypeBuilder<Resto> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.BuildingId)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(50);

        builder.Property(x => x.Email)
            .HasMaxLength(250);

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(500);

        // Configure OpeningHours as JSON column (TEXT for SQLite compatibility)
        builder.Property(x => x.OpeningHours)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<DayOfWeek, string>>(v, (JsonSerializerOptions?)null))
            .HasColumnType("TEXT");

        // Configure KitchenType as JSON column (TEXT for SQLite compatibility)
        builder.Property(x => x.KitchenType)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null))
            .HasColumnType("TEXT");
    }

}
