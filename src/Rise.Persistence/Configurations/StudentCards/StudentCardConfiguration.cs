using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.StudentCards;

namespace Rise.Persistence.Configurations.StudentCards;

/// <summary>
/// Configuration for <see cref="StudentCard"/>.
/// </summary>
internal class StudentCardConfiguration : EntityConfiguration<StudentCard>
{
    public override void Configure(EntityTypeBuilder<StudentCard> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.PersonalNumber)
            .IsRequired()
            .HasMaxLength(9);

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.BirthDate)
            .IsRequired();

        builder.Property(x => x.ExpirationDate)
            .IsRequired();

        builder.Property(x => x.ProfilePicture)
            .HasMaxLength(500);

        // Create a unique index on PersonalNumber
        builder.HasIndex(x => x.PersonalNumber)
            .IsUnique();

        // Configure foreign key relationship to IdentityUser
        builder.HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
