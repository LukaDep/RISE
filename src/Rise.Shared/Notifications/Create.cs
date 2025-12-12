namespace Rise.Shared.Notifications;

/// <summary>
/// Request wrappers for push subscription operations.
/// </summary>
public static partial class PushSubscriptionRequest
{
    /// <summary>
    /// Request to create or update a push subscription.
    /// Contains the subscription endpoint and encryption keys required for web push notifications.
    /// </summary>
    public class Create
    {
        public required string Endpoint { get; set; }
        public string? ExpirationTime { get; set; }
        public required Keys Keys { get; set; }

        public class Validator : AbstractValidator<Create>
        {
            public Validator()
            {
                RuleFor(x => x.Endpoint)
                  .NotEmpty()
                  .WithMessage("Endpoint mag niet leeg zijn.");

                RuleFor(x => x.Keys)
                  .NotNull()
                  .WithMessage("Keys mogen niet leeg zijn.");

                RuleFor(x => x.Keys.P256dh)
                  .NotEmpty()
                  .WithMessage("P256dh key mag niet leeg zijn.");

                RuleFor(x => x.Keys.Auth)
                  .NotEmpty()
                  .WithMessage("Auth key mag niet leeg zijn.");
            }
        }
    }

    public class Keys
    {
        public required string P256dh { get; set; }
        public required string Auth { get; set; }
    }
}