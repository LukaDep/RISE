namespace Rise.Shared.Notifications;

/// <summary>
/// Request wrappers for push notification operations.
/// </summary>
public static partial class Push
{
    /// <summary>
    /// Supported notification types for categorization.
    /// </summary>
    public static class NotificationTypes
    {
        public const string Grades = "grades";
        public const string Schedule = "schedule";
        public const string Campus = "campus";
        public const string News = "news";
    }

    /// <summary>
    /// Request to send a push notification.
    /// Contains notification content (title, body, url) and optional targeting (userGuid) and categorization (notificationType).
    /// </summary>
    public class Send
    {
        public Guid? userGuid { get; set; }
        public required string title { get; set; }
        public required string body { get; set; }
        public string? url { get; set; }

        /// <summary>
        /// The type/category of the notification (e.g., "grades", "schedule", "campus", "news").
        /// Use Push.NotificationTypes constants for valid values.
        /// </summary>
        public string? notificationType { get; set; }

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