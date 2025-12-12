using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Rise.Persistence.Configurations.Deadlines;

/// <summary>
/// Specific configuration for <see cref="Deadlines"/>.
/// Configures property constraints and foreign key relationship to IdentityUser.
/// </summary>
internal class DeadlineConfiguration : EntityConfiguration<Domain.Deadlines.Deadline>
{
    /// <summary>
    /// Configures the Deadline entity properties including title, dates, and user foreign key with cascade delete.
    /// </summary>
    /// <param name="builder">The entity type builder for Deadline.</param>
    public override void Configure(EntityTypeBuilder<Domain.Deadlines.Deadline> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.UserId)
            .IsRequired();

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

        // Configure foreign key relationship to IdentityUser
        builder.HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Create index on UserId for faster queries
        builder.HasIndex(x => x.UserId);
    }
}