using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Common;
using Rise.Persistence.ValueGenerators;

namespace Rise.Persistence.Configurations;

/// <summary>
/// Base configuration for an <see cref="Entity"/>
/// </summary>
/// <typeparam name="TEntity"></typeparam>
internal class EntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : Entity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // All tables are singlular named and have the name of the class e.g. Product instead of Products.
        builder.ToTable(typeof(TEntity).Name);

        // Configure Id as UUIDv7 (stored as CHAR(36) for MySQL/MariaDB compatibility)
        builder.Property(e => e.Id)
            .HasMaxLength(36)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<UuidV7ValueGenerator>();

        // CreatedAt should be filled in by the database when using raw SQL.
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)"); // MariaDB/MySQL specific

        // UpdatedAt should be filled in by the database when using raw SQL.
        builder.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)"); // MariaDB/MySQL specific

        // IsDeleted should be false by default, used for softdelete.
        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);
    }
}