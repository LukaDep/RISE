namespace Rise.Domain.Notifications;

public class PushSubscriptions : Entity
{
    public Guid? UserId { get; set; }
    public required string Endpoint { get; set; }
    public required string P256dhKey { get; set; }
    public required string AuthKey { get; set; }
    public DateTime? LastUsedAt { get; set; }
}
