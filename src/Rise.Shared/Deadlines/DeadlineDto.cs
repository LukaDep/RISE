namespace Rise.Shared.Deadlines;

public static class DeadlineDto
{
    public class Index
    {
        public required Guid Id { get; set; }
        public required DateTime EndDate { get; set; }
        public required string Lector { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? Course { get; set; }
    }
}