using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Rise.Persistence.Configurations.Deadlines;

/// <summary>
/// Specific configuration for <see cref="Deadlines"/>.
/// </summary>
internal class DeadlineConfiguration: EntityConfiguration<Domain.Deadlines.Deadline>
{
    public override void Configure(EntityTypeBuilder<Domain.Deadlines.Deadline> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.EndDate)
            .IsRequired();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(x => x.Lector)
            .IsRequired()
            .HasMaxLength(40);
        builder.Property(x => x.Description)
            .HasMaxLength(250);
    }
}