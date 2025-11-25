namespace Rise.Shared.Notifications;

/// <summary>
/// Represents a static utility class containing request-related structures for products.
/// </summary>
public static partial class Push
{
    public class Send
    {
        public Guid? userGuid { get; set; }
        public required string title { get; set; }
        public required string body { get; set; }

        public class Validator : AbstractValidator<Send>
        {
            public Validator()
            {
                RuleFor(x => x.title)
                  .NotNull()
                  .WithMessage("Gelieve een titel te geven aan de notificatie.");

                RuleFor(x => x.body)
                  .NotEmpty()
                  .WithMessage("Gelieve een body mee te geven voor de notificatie.");
            }
        }
    }
}