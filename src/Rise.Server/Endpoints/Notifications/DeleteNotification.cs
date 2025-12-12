using FluentValidation;
using Rise.Shared.Notifications;

namespace Rise.Server.Endpoints.Notifications;

/// <summary>
/// Request DTO voor het verwijderen van een notificatie.
/// </summary>
public class DeleteNotificationRequest
{
    public Guid NotificationId { get; set; }

    public class Validator : AbstractValidator<DeleteNotificationRequest>
    {
        public Validator()
        {
            RuleFor(x => x.NotificationId)
                .NotEmpty()
                .WithMessage("Notificatie ID is verplicht.");
        }
    }
}

/// <summary>
/// Endpoint om een notificatie te verwijderen.
/// </summary>
public class DeleteNotification(ISentNotificationService sentNotificationService)
    : Endpoint<DeleteNotificationRequest, Result>
{
    /// <summary>
    /// Configures the endpoint route and authorization.
    /// </summary>
    public override void Configure()
    {
        Delete("/api/notifications/{NotificationId}");
        AllowAnonymous();
    }

    /// <summary>
    /// Deletes a specific notification by its ID.
    /// </summary>
    /// <param name="req">The request containing the notification ID to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating success or failure.</returns>
    public override async Task<Result> ExecuteAsync(DeleteNotificationRequest req, CancellationToken ct)
    {
        return await sentNotificationService.DeleteNotificationAsync(req.NotificationId, ct);
    }
}
