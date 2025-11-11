namespace Rise.Shared.Contact;

public static class ContactDto
{
    public class Index
    {
        public required Guid Id { get; set; }
        public required string Type { get; set; }
        public required string Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ContactPerson { get; set; }
        public IEnumerable<string>? Campusses { get; set; }
    }
}
