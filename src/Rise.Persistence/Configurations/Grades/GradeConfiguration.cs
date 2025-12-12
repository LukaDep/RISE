using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Grades;

namespace Rise.Persistence.Configurations.Grades;

/// <summary>
/// Specific configuration for <see cref="Grade"/>.
/// Configures property constraints for student grade records.
/// </summary>
internal class GradeConfiguration : EntityConfiguration<Grade>
{
    /// <summary>
    /// Configures the Grade entity properties including name, activity type, course info, and user ID.
    /// </summary>
    /// <param name="builder">The entity type builder for Grade.</param>
    public override void Configure(EntityTypeBuilder<Grade> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.ActivityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Feedback)
            .HasMaxLength(2000);

        builder.Property(x => x.Date)
            .IsRequired();

        builder.Property(x => x.CourseId)
            .HasMaxLength(250);

        builder.Property(x => x.CourseName)
            .HasMaxLength(250);

        builder.Property(x => x.Year)
            .HasMaxLength(50);

        builder.Property(uw => uw.UserId)
            .IsRequired();
    }
}
