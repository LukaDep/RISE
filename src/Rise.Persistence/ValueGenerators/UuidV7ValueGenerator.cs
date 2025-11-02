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
    return GenerateUuidV7();
  }

  /// <summary>
  /// Generates a UUIDv7 string.
  /// Format: xxxxxxxx-xxxx-7xxx-xxxx-xxxxxxxxxxxx
  /// Where the first 48 bits are a Unix timestamp in milliseconds,
  /// followed by version bits (0111), and random bits.
  /// </summary>
  public static string GenerateUuidV7()
  {
    // Get current Unix timestamp in milliseconds
    long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    // Create a byte array for the UUID (16 bytes)
    byte[] uuidBytes = new byte[16];

    // Fill with random bytes
    Random.Shared.NextBytes(uuidBytes);

    // Set timestamp (first 48 bits / 6 bytes)
    uuidBytes[0] = (byte)(timestamp >> 40);
    uuidBytes[1] = (byte)(timestamp >> 32);
    uuidBytes[2] = (byte)(timestamp >> 24);
    uuidBytes[3] = (byte)(timestamp >> 16);
    uuidBytes[4] = (byte)(timestamp >> 8);
    uuidBytes[5] = (byte)timestamp;

    // Set version to 7 (0111 in binary, so 0x70 with mask 0x0f for random bits)
    uuidBytes[6] = (byte)((uuidBytes[6] & 0x0f) | 0x70);

    // Set variant to 10xx (RFC 4122)
    uuidBytes[8] = (byte)((uuidBytes[8] & 0x3f) | 0x80);

    // Convert to GUID and then to string
    var guid = new Guid(uuidBytes);
    return guid.ToString();
  }
}
