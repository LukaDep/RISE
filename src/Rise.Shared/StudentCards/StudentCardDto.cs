using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Rise.Shared.StudentCards;

/// <summary>
/// Data Transfer Object for Student Card information.
/// </summary>
public class StudentCardDto
{
    /// <summary>
    /// Personal number - a 9-digit identification number.
    /// </summary>
    [JsonPropertyName("personalNumber")]
    public string PersonalNumber { get; set; } = default!;

    /// <summary>
    /// Student's first name.
    /// </summary>
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = default!;

    /// <summary>
    /// Student's last name.
    /// </summary>
    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = default!;

    /// <summary>
    /// Expiration date of the student card.
    /// </summary>
    [JsonPropertyName("expirationDate")]
    public DateTime ExpirationDate { get; set; }

    /// <summary>
    /// URL or path to the student's profile picture.
    /// </summary>
    public string? ProfilePicture { get; set; }

    /// <summary>
    /// Indicates whether the card is currently valid (not expired).
    /// </summary>
    [JsonPropertyName("isValid")]
    public bool IsValid { get; set; }

    /// <summary>
    /// Converts the student card data to a JSON object representation.
    /// </summary>
    public JsonObject ToJsonObject()
    {
        var json = JsonSerializer.SerializeToNode(this)?.AsObject();
        json?.Remove("ProfilePicture");
        return json!;
    }
}
