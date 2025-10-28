namespace Rise.Shared.Contact;

public static class ContactDto
{
    public class Index
    {
        public required int Id { get; set; }
        public required string Type { get; set; }
        public required string Name { get; set; }
        public string? Email { get; set; }
        public string? phoneNumber { get; set; }
        public string? ContactPerson { get; set; }
        public List<int>? Campusses { get; set; }
    }
}
