using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Events;

namespace Rise.Persistence.Configurations.Events;

internal class EventConfiguration : EntityConfiguration<Event>
{
    public override void Configure(EntityTypeBuilder<Event> builder)
    {
        base.Configure(builder);

        builder.ToTable("Events");

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Location)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.RegistrationLink)
            .HasMaxLength(1000);

        builder.Property(x => x.StartDateTime)
            .IsRequired()
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(x => x.EndDateTime)
            .IsRequired()
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }
}
