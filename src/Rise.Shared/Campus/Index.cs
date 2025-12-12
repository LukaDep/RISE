namespace Rise.Shared.Campus;

/// <summary>
/// Response wrappers for campus-related operations.
/// </summary>
public static partial class CampusResponse
{
    /// <summary>
    /// Response containing a list of campuses.
    /// Used for retrieving and displaying available campuses with their buildings.
    /// </summary>
    public class Index
    {
        public IEnumerable<CampusDto.Index> Campuses { get; set; } = [];
    }
}
