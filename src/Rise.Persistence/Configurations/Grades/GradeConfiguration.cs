using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Grades;

namespace Rise.Persistence.Configurations.Grades;

/// <summary>
/// Specific configuration for <see cref="Grade"/>.
/// </summary>
internal class GradeConfiguration : EntityConfiguration<Grade>
{
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
    }
}
