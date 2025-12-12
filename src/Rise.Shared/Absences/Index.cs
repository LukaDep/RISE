namespace Rise.Shared.Absences;

/// <summary>
/// Response wrappers for absence-related operations.
/// </summary>
public static partial class AbsencesResponse
{
    /// <summary>
    /// Response containing a list of absences.
    /// Used for paginated absence queries.
    /// </summary>
    public class Index
    {
        public IEnumerable<AbsenceDto.Index> Absences { get; set; } = [];
    }
}