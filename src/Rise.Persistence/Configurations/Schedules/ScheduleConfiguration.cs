using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Schedules;

namespace Rise.Persistence.Configurations.Schedules;

/// <summary>
/// Specific configuration for <see cref="Schedule"/>.
/// </summary>
internal class ScheduleConfiguration : EntityConfiguration<Schedule>
{
    public override void Configure(EntityTypeBuilder<Schedule> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.StartDateTime)
            .IsRequired();

        builder.Property(x => x.EndDateTime)
            .IsRequired();

        builder.Property(x => x.Course)
            .IsRequired()
            .HasMaxLength(511);

        builder.Property(x => x.WorkForm)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Environment)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Room)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Teacher)
            .IsRequired()
            .HasMaxLength(255);
    }
}
