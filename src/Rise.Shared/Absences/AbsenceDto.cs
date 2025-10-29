namespace Rise.Shared.Absences;

public static class AbsenceDto
{
    public class Index
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required string Reason { get; set; }
    }
}