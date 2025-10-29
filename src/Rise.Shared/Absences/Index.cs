namespace Rise.Shared.Absences;

public static partial class AbsencesResponse
{
    public class Index
    {
        public IEnumerable<AbsenceDto.Index> Absences { get; set; } = [];
    }
}