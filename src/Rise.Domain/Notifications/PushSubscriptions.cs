namespace Rise.Domain.Notifications;

/// <summary>
/// Represents a push notification subscription for a user's device.
/// Contains the endpoint URL and encryption keys required for web push notifications.
/// </summary>
public class PushSubscriptions : Entity
{
    /// <summary>
    /// The unique identifier of the user who owns this subscription.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// The push service endpoint URL.
    /// </summary>
    public required string Endpoint { get; set; }

    /// <summary>
    /// The P-256 Diffie-Hellman public key for encryption.
    /// </summary>
    public required string P256dhKey { get; set; }

    /// <summary>
    /// The authentication secret key.
    /// </summary>
    public required string AuthKey { get; set; }

    /// <summary>
    /// The date and time when this subscription was last used.
    /// </summary>
    public DateTime? LastUsedAt { get; set; }
}
