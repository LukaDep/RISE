namespace Rise.Shared.StudentCards;

/// <summary>
/// Data Transfer Object for Student Card information.
/// </summary>
public class StudentCardDto
{
    /// <summary>
    /// Personal number - a 9-digit identification number.
    /// </summary>
    public string PersonalNumber { get; set; } = default!;

    /// <summary>
    /// Student's first name.
    /// </summary>
    public string FirstName { get; set; } = default!;

    /// <summary>
    /// Student's last name.
    /// </summary>
    public string LastName { get; set; } = default!;

    /// <summary>
    /// Expiration date of the student card.
    /// </summary>
    public DateTime ExpirationDate { get; set; }

    /// <summary>
    /// URL or path to the student's profile picture.
    /// </summary>
    public string? ProfilePicture { get; set; }

    /// <summary>
    /// Indicates whether the card is currently valid (not expired).
    /// </summary>
    public bool IsValid { get; set; }
}
