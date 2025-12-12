namespace Rise.Shared.Campus;

/// <summary>
/// Response wrappers for building-related operations.
/// </summary>
public static partial class BuildingResponse
{
    /// <summary>
    /// Response containing a single building.
    /// Used for detail views of individual buildings.
    /// </summary>
    public class Get
    {
        public required BuildingDto.Index Building { get; set; }
    }
}