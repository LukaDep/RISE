using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Notifications;
using Rise.Persistence;
using Rise.Services.Identity;
using Rise.Shared.Identity;
using Rise.Shared.Notifications;
using Serilog;
using WebPush;

namespace Rise.Services.Notifications;

/// <summary>
/// Service for managing notification preferences and push subscriptions.
/// </summary>
public class NotificationPreferencesService(ApplicationDbContext dbContext, ISessionContextProvider sessionContextProvider, ISentNotificationService sentNotificationService) : INotificationPreferencesService
{
    /// <summary>
    /// Retrieves the notification preferences for the current user.
    /// Creates default preferences if they don't exist yet.
    /// </summary>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result with NotificationPreferencesResponse.Index containing preferences and subscription status, or Unauthorized if not logged in</returns>
    public async Task<Result<NotificationPreferencesResponse.Index>> GetUserPreferencesByIdAsync(CancellationToken ctx = default)
    {
        try
        {
            var userGuid = sessionContextProvider.User?.GetUserId();
            if (userGuid == null)
                return Result.Unauthorized("Gebruiker niet ingelogd.");

            var userData = await dbContext.PushSubscriptions
                .Where(s => s.UserId == userGuid)
                .Join(
                    dbContext.NotificationPreferences,
                    s => s.UserId,
                    p => p.Id,
                    (s, p) => new { Subscription = s, Preferences = p }
                )
                .FirstOrDefaultAsync(ctx);

            NotificationPreferences preferences;
            PushSubscriptions subscription;

            // Geen record → defaults aanmaken
            if (userData == null || userData.Preferences == null)
            {
                preferences = new NotificationPreferences(userGuid.Value)
                {
                    GradesNotifications = true,
                    ScheduleNotifications = true,
                    CampusNotifications = true,
                    NewsNotifications = true
                };

                dbContext.NotificationPreferences.Add(preferences);
                await dbContext.SaveChangesAsync(ctx);

                dbContext.Entry(preferences).State = EntityState.Detached;
            }
            else
            {
                preferences = userData.Preferences;
            }

            // TODO: hoe moet ik hier dan langs backend side subscriben?
            if (userData == null || userData.Subscription == null)
            {
                subscription = new PushSubscriptions()
                {
                    UserId = preferences.Id,
                    Endpoint = string.Empty,
                    P256dhKey = string.Empty,
                    AuthKey = string.Empty
                };

                dbContext.PushSubscriptions.Add(subscription);
                await dbContext.SaveChangesAsync(ctx);

                dbContext.Entry(subscription).State = EntityState.Detached;
            }
            else
            {
                subscription = userData.Subscription;
            }


            var dto = new NotificationPreferencesDTO.Index
            {
                UserId = preferences.Id,
                GradesNotifications = preferences.GradesNotifications,
                ScheduleNotifications = preferences.ScheduleNotifications,
                CampusNotifications = preferences.CampusNotifications,
                NewsNotifications = preferences.NewsNotifications,
                IsEnabled = subscription is not null
            };

            return Result.Success(new NotificationPreferencesResponse.Index { NotificationPreference = dto });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fout in GetUserPreferencesByIdAsync");
            return Result.Error("Er ging iets fout bij het ophalen van notificatievoorkeuren.");
        }
    }

    /// <summary>
    /// Updates the notification preferences for the current user.
    /// </summary>
    /// <param name="req">NotificationPreferencesRequest.Edit with the new preference settings</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result.Success on successful update, Unauthorized if not logged in, NotFound if preferences don't exist</returns>
    public async Task<Result> EditAsync(NotificationPreferencesRequest.Edit req, CancellationToken ctx = default)
    {
        try
        {
            var userGuid = sessionContextProvider.User?.GetUserId();
            if (userGuid == null)
                return Result.Unauthorized("Gebruiker niet ingelogd.");

            var preferences = await dbContext.NotificationPreferences.SingleOrDefaultAsync(np => np.Id == userGuid, ctx);

            if (preferences is null)
                return Result.NotFound($"Geen notificatievoorkeuren gevonden voor gebruiker '{userGuid}'.");

            preferences.GradesNotifications = req.GradesNotifications;
            preferences.ScheduleNotifications = req.ScheduleNotifications;
            preferences.CampusNotifications = req.CampusNotifications;
            preferences.NewsNotifications = req.NewsNotifications;

            await dbContext.SaveChangesAsync(ctx);
            return Result.Success();
        }
        catch (DbUpdateException dbEx)
        {
            Log.Error(dbEx, "Databasefout bij het aanpassen van notificatievoorkeuren");
            return Result.Error("Kon notificatievoorkeuren niet opslaan.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Onbekende fout in EditAsync");
            return Result.Error("Er ging iets onverwachts fout bij het opslaan.");
        }
    }

    /// <summary>
    /// Registers or updates a push subscription for the current user.
    /// </summary>
    /// <param name="req">PushSubscriptionRequest.Create with endpoint and keys</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result.Success on successful registration, Unauthorized if not logged in</returns>
    public async Task<Result> Subscribe(PushSubscriptionRequest.Create req, CancellationToken ctx = default)
    {
        try
        {
            var userGuid = sessionContextProvider.User?.GetUserId();

            var existing = await dbContext.PushSubscriptions
                .FirstOrDefaultAsync(x => x.Endpoint == req.Endpoint, ctx);

            if (existing == null)
            {
                var entity = new PushSubscriptions
                {
                    UserId = userGuid,
                    Endpoint = req.Endpoint,
                    P256dhKey = req.Keys.P256dh,
                    AuthKey = req.Keys.Auth,
                };

                dbContext.PushSubscriptions.Add(entity);
            }
            else
            {
                existing.UserId = userGuid;
                existing.P256dhKey = req.Keys.P256dh;
                existing.AuthKey = req.Keys.Auth;
                existing.LastUsedAt = DateTime.Now;
            }

            await dbContext.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fout in Subscribe()");
            return Result.Error("Kon push subscription niet opslaan.");
        }
    }

    /// <summary>
    /// Removes all push subscriptions for the current user.
    /// </summary>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result.Success on successful removal, Unauthorized if not logged in</returns>
    public async Task<Result> Unsubscribe(CancellationToken ctx = default)
    {
        try
        {
            var userGuid = sessionContextProvider.User?.GetUserId();
            if (userGuid == null)
                return Result.Unauthorized("Gebruiker niet ingelogd.");

            await dbContext.PushSubscriptions
                .Where(x => x.UserId == userGuid.Value)
                .ExecuteDeleteAsync(ctx);

            return Result.Success();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fout in Unsubscribe()");
            return Result.Error("Kon push subscription niet verwijderen.");
        }
    }

    /// <summary>
    /// Sends a test push notification to a specific user or to all users.
    /// Saves the sent notification with delivery status.
    /// </summary>
    /// <param name="req">Push.Send with userGuid (null for all users), title, body, url and notificationType</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result.Success on successful send</returns>
    public async Task<Result> SendTestToUser(Push.Send req, CancellationToken ctx = default)
    {
        try
        {
            if (req.userGuid == null)
                return await SendTestToAllUsers(req.title, req.body, req.url, req.notificationType);

            var userSubs = await dbContext.PushSubscriptions
                .Where(s => s.UserId == req.userGuid)
                .GroupJoin(
                    dbContext.NotificationPreferences,
                    s => s.UserId,
                    p => p.Id,
                    (s, prefs) => new { Subscription = s, Preferences = prefs.FirstOrDefault() }
                )
                .ToListAsync(ctx);

            if (!userSubs.Any())
            {
                Log.Warning("Geen push subscription gevonden voor gebruiker {UserId}", req.userGuid);
                return Result.Error("Geen push subscription gevonden voor deze gebruiker.");
            }

            foreach (var s in userSubs)
            {
                var sub = new PushSubscription(
                    s.Subscription.Endpoint,
                    s.Subscription.P256dhKey,
                    s.Subscription.AuthKey
                );

                var sendSuccess = await SendToSubscription(sub, req.title, req.body, req.url);
                var deliveryStatus = sendSuccess ? DeliveryStatus.Delivered : DeliveryStatus.Failed;

                await sentNotificationService.SaveSentNotificationAsync(
                    s.Subscription.Id,
                    req.title,
                    req.body,
                    req.url,
                    req.notificationType,
                    deliveryStatus,
                    ctx
                );
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fout in SendTestToUser() voor gebruiker {UserGuid}", req.userGuid);
            return Result.Error("Kon geen testmelding sturen.");
        }
    }

    /// <summary>
    /// Sends a push notification to a specific subscription via WebPush.
    /// </summary>
    /// <param name="sub">The PushSubscription to send to</param>
    /// <param name="title">The notification title</param>
    /// <param name="body">The notification body</param>
    /// <param name="url">Optional URL to open when clicking the notification</param>
    /// <returns>True on successful send, false on failure or invalid subscription</returns>
    public async Task<bool> SendToSubscription(PushSubscription sub, string title, string body, string? url = null)
    {
        var client = new WebPushClient();

        var vapid = new VapidDetails(
            "mailto:igorcasteleyn@gmail.com",
            "BCW-qlnpFfIjUDSN5cg0JUah1ktLevpGuU0HgBLvj76rpPinTndjtmEjZriWPsooBzKIJ4oEsTs8c1yAyCHBwGI",
            "9UbL0rmtrGs-uXRiiuXEVWD3MpH-DltRQpu7Yi92gpA"
        );

        var payloadObject = new Dictionary<string, object>
        {
            ["title"] = title,
            ["body"] = body
        };

        if (!string.IsNullOrEmpty(url))
            payloadObject["url"] = url;

        var payload = JsonSerializer.Serialize(payloadObject);

        try
        {
            await client.SendNotificationAsync(sub, payload, vapid);
            return true;
        }
        catch (WebPushException ex)
        {
            Log.Warning("Push failed voor {Endpoint}. Reden: {Message}. StatusCode: {StatusCode}", sub.Endpoint, ex.Message, ex.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Onbekende fout bij versturen van pushbericht naar {Endpoint}", sub.Endpoint);
            return false;
        }
    }

    /// <summary>
    /// Sends a push notification to all users with an active subscription.
    /// Saves the sent notification for each user with delivery status.
    /// </summary>
    /// <param name="title">The notification title</param>
    /// <param name="body">The notification body</param>
    /// <param name="url">Optional URL to open when clicking the notification</param>
    /// <param name="notificationType">Optional notification type</param>
    /// <returns>Result.Success after sending to all users</returns>
    public async Task<Result> SendTestToAllUsers(string title, string body, string? url = null, string? notificationType = null)
    {
        try
        {
            var subs = await dbContext.PushSubscriptions
                .GroupJoin(
                    dbContext.NotificationPreferences,
                    s => s.UserId,
                    p => p.Id,
                    (s, prefs) => new { Subscription = s, Preferences = prefs.FirstOrDefault() }
                )
                .ToListAsync();

            foreach (var s in subs)
            {
                var webPushSub = new PushSubscription(
                    s.Subscription.Endpoint,
                    s.Subscription.P256dhKey,
                    s.Subscription.AuthKey
                );

                var sendSuccess = await SendToSubscription(webPushSub, title, body, url);
                var deliveryStatus = sendSuccess ? DeliveryStatus.Delivered : DeliveryStatus.Failed;

                await sentNotificationService.SaveSentNotificationAsync(
                    s.Subscription.Id,
                    title,
                    body,
                    url,
                    notificationType,
                    deliveryStatus
                );
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fout in SendTestToAllUsers()");
            return Result.Error("Er ging iets mis bij het sturen van meldingen naar alle gebruikers.");
        }
    }

    /// <summary>
    /// Not implemented, front end only logic  
    /// </summary>
    public Task<Result> SyncSubscriptionAsync(CancellationToken ctx = default)
    {
        throw new NotImplementedException();
    }
}
