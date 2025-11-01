using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Rise.Persistence.Configurations.Identity;

/// <summary>
/// Configuration for the Identity tables.
/// </summary>
internal class IdentityConfiguration :
    IEntityTypeConfiguration<IdentityUser>,
    IEntityTypeConfiguration<IdentityRole>,
    IEntityTypeConfiguration<IdentityUserRole<string>>,
    IEntityTypeConfiguration<IdentityUserClaim<string>>,
    IEntityTypeConfiguration<IdentityUserLogin<string>>,
    IEntityTypeConfiguration<IdentityRoleClaim<string>>,
    IEntityTypeConfiguration<IdentityUserToken<string>>
{
    // Configures the IdentityUser tables.
    // NOTE:
    // If you want to use a separate schema for your Identity tables,
    // you can specify the schema as "auth" like this:
    //     builder.ToTable("Users", "auth");
    // However, be aware that SQLite does NOT support schemas, so this only works with SQL Server, MarioDB, PostgreSQL,...
    // The default below will work on any provider.

    public void Configure(EntityTypeBuilder<IdentityUser> builder)
    {
        builder.ToTable("Users");
        builder.Property(u => u.Id).HasMaxLength(255);
        builder.Property(u => u.NormalizedUserName).HasMaxLength(256);
        builder.Property(u => u.NormalizedEmail).HasMaxLength(256);
    }

    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.ToTable("Roles");
        builder.Property(r => r.Id).HasMaxLength(255);
        builder.Property(r => r.NormalizedName).HasMaxLength(256);
    }

    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        builder.ToTable("UserRoles");
        builder.Property(ur => ur.UserId).HasMaxLength(255);
        builder.Property(ur => ur.RoleId).HasMaxLength(255);
    }

    public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
    {
        builder.ToTable("UserClaims");
        builder.Property(uc => uc.UserId).HasMaxLength(255);
    }

    public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> builder)
    {
        builder.ToTable("UserLogins");
        builder.Property(ul => ul.LoginProvider).HasMaxLength(255);
        builder.Property(ul => ul.ProviderKey).HasMaxLength(255);
        builder.Property(ul => ul.UserId).HasMaxLength(255);
    }

    public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
    {
        builder.ToTable("RoleClaims");
        builder.Property(rc => rc.RoleId).HasMaxLength(255);
    }

    public void Configure(EntityTypeBuilder<IdentityUserToken<string>> builder)
    {
        builder.ToTable("UserTokens");
        builder.Property(ut => ut.UserId).HasMaxLength(255);
        builder.Property(ut => ut.LoginProvider).HasMaxLength(255);
        builder.Property(ut => ut.Name).HasMaxLength(255);
    }
}