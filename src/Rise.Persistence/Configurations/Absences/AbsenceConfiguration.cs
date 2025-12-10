using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Absences;
using Microsoft.EntityFrameworkCore;

namespace Rise.Persistence.Configurations.Absences;

/// <summary>
/// Specific configuration for <see cref="Absence"/>.
/// </summary>
internal class AbsenceConfiguration : EntityConfiguration<Absence>
{
    public override void Configure(EntityTypeBuilder<Absence> builder)
    {


        base.Configure(builder);

        builder.ToTable("Absence");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate)
            .IsRequired();

        builder.Property(x => x.Reason)
            .IsRequired()
            .HasMaxLength(1000);
    }
}
