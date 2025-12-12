namespace Rise.Shared.Campus;

/// <summary>
/// Response wrappers for campus-related operations.
/// </summary>
public static partial class CampusResponse
{
    /// <summary>
    /// Response containing a single campus.
    /// Used for detail views of individual campuses.
    /// </summary>
    public class Get
    {
        public required CampusDto.Index Campus { get; set; }
    }
}