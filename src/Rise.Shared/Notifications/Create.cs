namespace Rise.Shared.Notifications;

/// <summary>
/// Represents a static utility class containing request-related structures for products.
/// </summary>
public static partial class PushSubscriptionRequest
{
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