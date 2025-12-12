using Microsoft.EntityFrameworkCore;
using Rise.Domain.Notifications;
using Rise.Persistence;
using Rise.Services.Identity;
using Rise.Shared.Identity;
using Rise.Shared.Notifications;
using Serilog;

namespace Rise.Services.Notifications;

/// <summary>
/// Service for managing sent notifications.
/// </summary>
public class SentNotificationService(ApplicationDbContext dbContext, ISessionContextProvider sessionContextProvider) : ISentNotificationService
{
    /// <summary>
    /// Retrieves a paginated list of notifications for the current user.
    /// Includes total count and unread count.
    /// </summary>
    /// <param name="page">Page number (default 1)</param>
    /// <param name="pageSize">Number of items per page (default 20)</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result with SentNotificationResponse.Index containing notifications and counts, or Unauthorized if not logged in</returns>
    public async Task<Result<SentNotificationResponse.Index>> GetUserNotificationsAsync(int page = 1, int pageSize = 20, CancellationToken ctx = default)
    {
        try
        {
            var userGuid = sessionContextProvider.User?.GetUserId();
            if (userGuid == null)
                return Result.Unauthorized("Gebruiker niet ingelogd.");

            var query = dbContext.SentNotifications
                .Where(n => n.UserId == userGuid && !n.IsDeleted)
                .OrderByDescending(n => n.SentAt);

            var totalCount = await query.CountAsync(ctx);
            var unreadCount = await query.Where(n => !n.IsRead).CountAsync(ctx);

            var notifications = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new SentNotificationDTO.Index
                {
                    Id = n.Id,
                    Title = n.Title,
                    Body = n.Body,
                    Url = n.Url,
                    NotificationType = n.NotificationType,
                    SentAt = n.SentAt,
                    IsRead = n.IsRead,
                    ReadAt = n.ReadAt
                })
                .ToListAsync(ctx);

            return Result.Success(new SentNotificationResponse.Index
            {
                Notifications = notifications,
                TotalCount = totalCount,
                UnreadCount = unreadCount
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fout bij ophalen van notificaties");
            return Result.Error("Er ging iets fout bij het ophalen van notificaties.");
        }
    }

    /// <summary>
    /// Marks a specific notification as read for the current user.
    /// </summary>
    /// <param name="req">SentNotificationRequest.MarkAsRead with NotificationId</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result.Success on successful update, Unauthorized if not logged in, NotFound if notification doesn't exist</returns>
    public async Task<Result> MarkAsReadAsync(SentNotificationRequest.MarkAsRead req, CancellationToken ctx = default)
    {
        try
        {
            var userGuid = sessionContextProvider.User?.GetUserId();
            if (userGuid == null)
                return Result.Unauthorized("Gebruiker niet ingelogd.");

            var notification = await dbContext.SentNotifications
                .FirstOrDefaultAsync(n => n.Id == req.NotificationId && n.UserId == userGuid, ctx);

            if (notification == null)
                return Result.NotFound("Notificatie niet gevonden.");

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.Now;
                await dbContext.SaveChangesAsync(ctx);
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fout bij markeren van notificatie als gelezen");
            return Result.Error("Kon notificatie niet markeren als gelezen.");
        }
    }

    /// <summary>
    /// Marks all unread notifications as read for the current user.
    /// </summary>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result.Success on successful update, Unauthorized if not logged in</returns>
    public async Task<Result> MarkAllAsReadAsync(CancellationToken ctx = default)
    {
        try
        {
            var userGuid = sessionContextProvider.User?.GetUserId();
            if (userGuid == null)
                return Result.Unauthorized("Gebruiker niet ingelogd.");

            var now = DateTime.Now;
            await dbContext.SentNotifications
                .Where(n => n.UserId == userGuid && !n.IsRead && !n.IsDeleted)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(n => n.IsRead, true)
                    .SetProperty(n => n.ReadAt, now), ctx);

            return Result.Success();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fout bij markeren van alle notificaties als gelezen");
            return Result.Error("Kon notificaties niet markeren als gelezen.");
        }
    }

    /// <summary>
    /// Retrieves the number of unread notifications for the current user.
    /// </summary>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result with integer count of unread notifications, or Unauthorized if not logged in</returns>
    public async Task<Result<int>> GetUnreadCountAsync(CancellationToken ctx = default)
    {
        try
        {
            var userGuid = sessionContextProvider.User?.GetUserId();
            if (userGuid == null)
                return Result.Unauthorized("Gebruiker niet ingelogd.");

            var count = await dbContext.SentNotifications
                .CountAsync(n => n.UserId == userGuid && !n.IsRead && !n.IsDeleted, ctx);

            return Result.Success(count);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fout bij ophalen van ongelezen aantal");
            return Result.Error("Kon ongelezen aantal niet ophalen.");
        }
    }

    /// <summary>
    /// Deletes a notification (soft delete) for the current user.
    /// </summary>
    /// <param name="notificationId">The Guid of the notification to delete</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result.Success on successful deletion, Unauthorized if not logged in, NotFound if notification doesn't exist</returns>
    public async Task<Result> DeleteNotificationAsync(Guid notificationId, CancellationToken ctx = default)
    {
        try
        {
            var userGuid = sessionContextProvider.User?.GetUserId();
            if (userGuid == null)
                return Result.Unauthorized("Gebruiker niet ingelogd.");

            var notification = await dbContext.SentNotifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userGuid, ctx);

            if (notification == null)
                return Result.NotFound("Notificatie niet gevonden.");

            notification.IsDeleted = true;
            await dbContext.SaveChangesAsync(ctx);

            return Result.Success();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fout bij verwijderen van notificatie");
            return Result.Error("Kon notificatie niet verwijderen.");
        }
    }

    /// <summary>
    /// Saves a sent notification to the database for a specific user.
    /// Used to track notification history.
    /// </summary>
    /// <param name="userId">The Guid of the user receiving the notification</param>
    /// <param name="title">The notification title</param>
    /// <param name="body">The notification body</param>
    /// <param name="url">Optional URL linked to the notification</param>
    /// <param name="notificationType">Optional notification type</param>
    /// <param name="deliveryStatus">Delivery status (default Pending)</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    public async Task SaveSentNotificationAsync(Guid pushSubscriptionId, string title, string body, string? url = null, string? notificationType = null, DeliveryStatus deliveryStatus = DeliveryStatus.Pending, CancellationToken ctx = default)
    {
        try
        {
            var subscription = await dbContext.PushSubscriptions
                .FirstOrDefaultAsync(ps => ps.Id == pushSubscriptionId, ctx);

            if (subscription == null)
            {
                Log.Warning("Push subscription {PushSubscriptionId} niet gevonden bij opslaan notificatie", pushSubscriptionId);
                return;
            }

            var notification = new SentNotification
            {
                PushSubscriptionId = pushSubscriptionId,
                UserId = subscription.UserId,
                Title = title,
                Body = body,
                Url = url,
                NotificationType = notificationType,
                SentAt = DateTime.Now,
                IsRead = false,
                DeliveryStatus = deliveryStatus
            };

            dbContext.SentNotifications.Add(notification);
            await dbContext.SaveChangesAsync(ctx);

            Log.Information("Notificatie opgeslagen voor subscription {PushSubscriptionId} (UserId: {UserId}): {Title} (DeliveryStatus: {DeliveryStatus})",
                pushSubscriptionId, subscription.UserId, title, deliveryStatus);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fout bij opslaan van verzonden notificatie voor subscription {PushSubscriptionId}", pushSubscriptionId);
        }
    }
}
