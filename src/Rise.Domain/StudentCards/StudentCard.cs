using Rise.Domain.Common;

namespace Rise.Domain.StudentCards;

/// <summary>
/// Represents a digital student card containing personal and identification information.
/// </summary>
public class StudentCard : Entity
{
    /// <summary>
    /// The associated user's unique identifier (references IdentityUser.Id).
    /// </summary>
    public string UserId { get; private set; } = default!;

    /// <summary>
    /// Personal number - a 9-digit identification number.
    /// </summary>
    public string PersonalNumber { get; private set; } = default!;

    /// <summary>
    /// Student's first name.
    /// </summary>
    public string FirstName { get; private set; } = default!;

    /// <summary>
    /// Student's last name.
    /// </summary>
    public string LastName { get; private set; } = default!;

    /// <summary>
    /// Student's date of birth.
    /// </summary>
    public DateTime BirthDate { get; private set; }

    /// <summary>
    /// Expiration date of the student card.
    /// </summary>
    public DateTime ExpirationDate { get; private set; }

    /// <summary>
    /// URL or path to the student's profile picture.
    /// </summary>
    public string? ProfilePicture { get; private set; }

    private StudentCard() { }

    /// <summary>
    /// Creates a new student card.
    /// </summary>
    public StudentCard(
        string userId,
        string personalNumber,
        string firstName,
        string lastName,
        DateTime birthDate,
        DateTime expirationDate,
        string? profilePicture = null)
    {
        UserId = userId;
        PersonalNumber = personalNumber;
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        ExpirationDate = expirationDate;
        ProfilePicture = profilePicture;
    }

    /// <summary>
    /// Checks if the student card is currently valid (not expired).
    /// </summary>
    public bool IsValid() => DateTime.UtcNow <= ExpirationDate;
}
