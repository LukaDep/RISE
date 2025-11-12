using Microsoft.EntityFrameworkCore;
using Rise.Domain.Notifications;
using Rise.Persistence;
using Rise.Services.Identity;
using Rise.Shared.Common;
using Rise.Shared.Identity;
using Rise.Shared.Notifications;
using Serilog;

namespace Rise.Services.Notifications;

public class NotificationPreferencesService(ApplicationDbContext dbContext, ISessionContextProvider sessionContextProvider) : INotificationPreferencesService
{
    public async Task<Result<NotificationPreferencesResponse.Index>> GetByUserIdAsync(CancellationToken ctx = default)
    {

        var userGuid = sessionContextProvider.User?.GetUserId();
        if (userGuid == null)
            return Result.Unauthorized("Gebruiker niet ingelogd.");


        // Haal de notification preferences op voor de gebruiker
        var preferences = await dbContext.NotificationPreferences
          .AsNoTracking()
          .FirstOrDefaultAsync(np => np.Id == userGuid, ctx);

        // Als er geen preferences zijn, maak dan een default record aan
        if (preferences == null)
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
            Log.Information("NotificationPreferences gevonden voor gebruiker {UserGuid}: Grades={Grades}, Schedule={Schedule}, Campus={Campus}, News={News}",
                userGuid, preferences.GradesNotifications, preferences.ScheduleNotifications,
                preferences.CampusNotifications, preferences.NewsNotifications);
        }

        var dto = new NotificationPreferencesDTO.Index
        {
            UserId = preferences.Id,
            GradesNotifications = preferences.GradesNotifications,
            ScheduleNotifications = preferences.ScheduleNotifications,
            CampusNotifications = preferences.CampusNotifications,
            NewsNotifications = preferences.NewsNotifications
        };

        var response = new NotificationPreferencesResponse.Index
        {
            NotificationPreference = dto
        };

        return Result.Success(response);
    }

    public async Task<Result> EditAsync(NotificationPreferencesRequest.Edit req, CancellationToken ctx = default)
    {
        var userGuid = sessionContextProvider.User?.GetUserId();
        if (userGuid == null)
            return Result.Unauthorized("Gebruiker niet ingelogd.");

        var preferences = await dbContext.NotificationPreferences
            .SingleOrDefaultAsync(np => np.Id == userGuid, ctx);

        if (preferences is null)
            return Result.NotFound($"Geen notificatievoorkeuren gevonden voor gebruiker '{userGuid}'.");

        preferences.GradesNotifications = req.GradesNotifications;
        preferences.ScheduleNotifications = req.ScheduleNotifications;
        preferences.CampusNotifications = req.CampusNotifications;
        preferences.NewsNotifications = req.NewsNotifications;

        await dbContext.SaveChangesAsync(ctx);
        return Result.Success();
    }

}
