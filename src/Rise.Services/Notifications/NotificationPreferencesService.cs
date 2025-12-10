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

public class NotificationPreferencesService(ApplicationDbContext dbContext, ISessionContextProvider sessionContextProvider, ISentNotificationService sentNotificationService) : INotificationPreferencesService
{
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

    public async Task<Result> SendTestToUser(Push.Send req, CancellationToken ctx = default)
    {
        try
        {
            if (req.userGuid == null)
                return await SendTestToAllUsers(req.title, req.body, req.url, req.notificationType);

            var userData = await dbContext.PushSubscriptions
                .Where(s => s.UserId == req.userGuid)
                .Join(
                    dbContext.NotificationPreferences,
                    s => s.UserId,
                    p => p.Id,
                    (s, p) => new { Subscription = s, Preferences = p }
                )
                .FirstOrDefaultAsync(ctx);

            DeliveryStatus deliveryStatus;

            if (userData == null)
            {
                // No push subscription found for this user
                Log.Warning("Geen push subscription gevonden voor gebruiker {UserId}", req.userGuid);
                return Result.Error("Geen push subscription gevonden voor deze gebruiker.");
            }

            var sub = new PushSubscription(
                userData.Subscription.Endpoint,
                userData.Subscription.P256dhKey,
                userData.Subscription.AuthKey
            );

            var sendSuccess = await SendToSubscription(sub, req.title, req.body, req.url);
            deliveryStatus = sendSuccess ? DeliveryStatus.Delivered : DeliveryStatus.Failed;

            // Save the sent notification with delivery status
            await sentNotificationService.SaveSentNotificationAsync(
                userData.Subscription.Id,
                req.title,
                req.body,
                req.url,
                req.notificationType,
                deliveryStatus,
                ctx
            );

            return Result.Success();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fout in SendTestToUser()");
            return Result.Error("Kon geen testmelding sturen.");
        }
    }

    /// <summary>
    /// Sends a push notification to a specific subscription.
    /// </summary>
    /// <param name="sub">The push subscription to send to.</param>
    /// <param name="title">The notification title.</param>
    /// <param name="body">The notification body.</param>
    /// <param name="url">Optional URL for the notification.</param>
    /// <returns>True if the notification was successfully sent, false otherwise.</returns>
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
            // Subscription no longer valid
            Log.Warning("Push failed voor {Endpoint}. Reden: {Message}", sub.Endpoint, ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Onbekende fout bij versturen van pushbericht");
            return false;
        }
    }

    public async Task<Result> SendTestToAllUsers(string title, string body, string? url = null, string? notificationType = null)
    {
        try
        {
            var subs = await dbContext.PushSubscriptions
                .Join(
                    dbContext.NotificationPreferences,
                    s => s.UserId,
                    p => p.Id,
                    (s, p) => new { Subscription = s, Preferences = p }
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

                // Save the sent notification for each subscription with delivery status
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
