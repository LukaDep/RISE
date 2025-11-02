using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Schedule;

namespace Rise.Persistence.Configurations.Schedule;

/// <summary>
/// Specific configuration for <see cref="Reservation"/>.
/// </summary>
internal class ReservationConfiguration : EntityConfiguration<Reservation>
{
  public override void Configure(EntityTypeBuilder<Reservation> builder)
  {
    base.Configure(builder);

    builder.Property(x => x.StartDateTime)
        .IsRequired();

    builder.Property(x => x.EndDateTime)
        .IsRequired();

    builder.Property(x => x.Course)
        .IsRequired()
        .HasMaxLength(250);

    builder.Property(x => x.WorkForm)
        .IsRequired()
        .HasMaxLength(100);

    builder.Property(x => x.Environment)
        .IsRequired()
        .HasMaxLength(250);

    builder.Property(x => x.Room)
        .IsRequired()
        .HasMaxLength(100);

    builder.Property(x => x.Teacher)
        .IsRequired()
        .HasMaxLength(250);
  }
}
