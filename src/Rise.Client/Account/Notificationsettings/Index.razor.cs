using Microsoft.AspNetCore.Components;
using Rise.Shared.Notifications;
using Serilog;

namespace Rise.Client.Account.Notificationsettings;

public partial class Index : ComponentBase
{
    [Inject]
    public required INotificationPreferencesService NotificationPreferencesService { get; set; }
    private NotificationPreferencesDTO.Index? notificationPreferences;
    private bool isLoading = true;
    private string? errorMessage;

    /// <summary>
    /// Local property voor two-way binding van de IsEnabled toggle.
    /// Zet alle notification preferences tegelijk aan of uit.
    /// </summary>
    private bool IsEnabled
    {
        get => notificationPreferences?.IsEnabled ?? false;
        set
        {
            if (notificationPreferences != null)
            {
                notificationPreferences.GradesNotifications = value;
                notificationPreferences.ScheduleNotifications = value;
                notificationPreferences.CampusNotifications = value;
                notificationPreferences.NewsNotifications = value;
            }
        }
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await LoadNotificationPreferencesAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fout bij het laden van notification preferences");
            errorMessage = "Er is een onverwachte fout opgetreden bij het laden van de instellingen.";
            isLoading = false;
        }
    }

    private async Task LoadNotificationPreferencesAsync()
    {
        isLoading = true;
        errorMessage = null;

        Log.Information("LoadNotificationPreferencesAsync gestart");

        var result = await NotificationPreferencesService.GetByUserIdAsync();

        Log.Information("Result ontvangen: IsSuccess={IsSuccess}, Result={HasValue}",
            result.IsSuccess, result.Value != null);

        if (result.IsSuccess && result.Value?.NotificationPreference != null)
        {
            notificationPreferences = result.Value.NotificationPreference;
            Log.Information("NotificationPreferences geladen: Grades={Grades}, Schedule={Schedule}, Campus={Campus}, News={News}",
                notificationPreferences.GradesNotifications,
                notificationPreferences.ScheduleNotifications,
                notificationPreferences.CampusNotifications,
                notificationPreferences.NewsNotifications);
        }
        else
        {
            errorMessage = "Kon notification preferences niet laden.";
            notificationPreferences = null;
            Log.Warning("Fout bij laden: IsSuccess={IsSuccess}, Value={Value}", result.IsSuccess, result.Value);
        }

        isLoading = false;
        Log.Information("Loading afgerond: isLoading={IsLoading}, hasPreferences={HasPreferences}",
            isLoading, notificationPreferences != null);
    }

    /// <summary>
    /// Wordt aangeroepen telkens een toggle verandert.
    /// </summary>
    private async Task OnNotificationChanged(string fieldName, bool value)
    {
        if (notificationPreferences == null)
            return;

        switch (fieldName)
        {
            case nameof(notificationPreferences.GradesNotifications):
                notificationPreferences.GradesNotifications = value;
                break;
            case nameof(notificationPreferences.ScheduleNotifications):
                notificationPreferences.ScheduleNotifications = value;
                break;
            case nameof(notificationPreferences.CampusNotifications):
                notificationPreferences.CampusNotifications = value;
                break;
            case nameof(notificationPreferences.NewsNotifications):
                notificationPreferences.NewsNotifications = value;
                break;
        }

        Log.Information("Notificatievoorkeur gewijzigd: {Field} = {Value}", fieldName, value);

        try
        {
            // Direct opslaan naar de backend
            var editRequest = new NotificationPreferencesRequest.Edit
            {
                GradesNotifications = notificationPreferences.GradesNotifications,
                ScheduleNotifications = notificationPreferences.ScheduleNotifications,
                CampusNotifications = notificationPreferences.CampusNotifications,
                NewsNotifications = notificationPreferences.NewsNotifications
            };

            var saveResult = await NotificationPreferencesService.EditAsync(editRequest, default);

            if (!saveResult.IsSuccess)
            {
                Log.Warning("Kon notificatievoorkeur niet opslaan: {Field}", fieldName);
                errorMessage = "Kon de wijziging niet opslaan. Probeer het opnieuw.";
            }
            else
            {
                Log.Information("Notificatievoorkeur succesvol opgeslagen: {Field}", fieldName);
                errorMessage = null;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fout bij opslaan van notificatievoorkeur: {Field}", fieldName);
            errorMessage = "Er is een fout opgetreden bij het opslaan. Probeer het opnieuw.";
        }
    }

    private void GoBack()
    {
        NavigationManager.NavigateTo("/account");
    }
}
