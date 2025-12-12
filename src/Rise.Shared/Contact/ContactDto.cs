namespace Rise.Shared.Contact;

/// <summary>
/// Data transfer objects for contact information.
/// </summary>
public static class ContactDto
{
    /// <summary>
    /// Represents a contact for display and retrieval.
    /// Contains contact details including type, name, email, phone, contact person, and associated campuses.
    /// </summary>
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
