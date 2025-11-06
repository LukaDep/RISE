using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Contact;

namespace Rise.Persistence.Configurations.Contact;

/// <summary>
/// Specific configuration for <see cref="Domain.Contact.Contact"/>.
/// </summary>
internal class ContactConfiguration : EntityConfiguration<Domain.Contact.Contact>
{
    public override void Configure(EntityTypeBuilder<Domain.Contact.Contact> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Email)
            .HasMaxLength(250);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(50);

        builder.Property(x => x.ContactPerson)
            .HasMaxLength(250);

        // Configure Campusses as JSON column
        builder.Property(x => x.Campusses)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null))
            .HasColumnType("json");
    }
}
