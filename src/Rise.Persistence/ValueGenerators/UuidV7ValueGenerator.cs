using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Rise.Persistence.ValueGenerators;

/// <summary>
/// Generates UUIDv7 values for entity IDs.
/// UUIDv7 is a time-ordered UUID that provides better database indexing performance.
/// </summary>
public class UuidV7ValueGenerator : ValueGenerator<string>
{

    public override bool GeneratesTemporaryValues => false;


    public override string Next(EntityEntry entry)
    {
        return Guid.CreateVersion7().ToString();
    }
}
