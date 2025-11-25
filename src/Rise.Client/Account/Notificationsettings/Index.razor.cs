using Microsoft.AspNetCore.Components;
using Rise.Shared.Notifications;
using Serilog;
using WebPush;

namespace Rise.Client.Account.Notificationsettings;

public partial class Index : ComponentBase
{
    [Inject]
    public required INotificationPreferencesService NotificationPreferencesService { get; set; }
    private NotificationPreferencesDTO.Index? notificationPreferences;
    private bool isLoading = true;
    private string? errorMessage;
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

        var result = await NotificationPreferencesService.GetUserPreferencesByIdAsync();


        if (result.IsSuccess && result.Value?.NotificationPreference != null)
        {
            notificationPreferences = result.Value.NotificationPreference;
        }
        else
        {
            errorMessage = "Kon notification preferences niet laden.";
            notificationPreferences = null;
            Log.Warning("Fout bij laden: IsSuccess={IsSuccess}, Value={Value}", result.IsSuccess, result.Value);
        }

        isLoading = false;
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
            case nameof(notificationPreferences.IsEnabled):
                notificationPreferences.IsEnabled = value;
                if (value)
                {
                    var subscribeResult = await NotificationPreferencesService.Subscribe(new PushSubscriptionRequest.Create //Tijdelijk een lege toegevoegd tot er een betere en propere manier word gevonden
                    {
                        Endpoint = string.Empty,
                        Keys = new PushSubscriptionRequest.Keys
                        {
                            P256dh = string.Empty,
                            Auth = string.Empty
                        }
                    });

                    if (!subscribeResult.IsSuccess)
                    {
                        var error = subscribeResult.Errors.FirstOrDefault() ?? "Onbekende fout";
                        Log.Warning("Fout bij aanmelden voor push-meldingen: {Error}", error);
                        errorMessage = error;
                        notificationPreferences.IsEnabled = false;
                        return;
                    }
                }
                else
                {
                    var unsubscribeResult = await NotificationPreferencesService.Unsubscribe();

                    if (!unsubscribeResult.IsSuccess)
                    {
                        var error = unsubscribeResult.Errors.FirstOrDefault() ?? "Onbekende fout";
                        Log.Warning("Fout bij afmelden voor push-meldingen: {Error}", error);
                        errorMessage = error;
                        notificationPreferences.IsEnabled = true;
                        return;
                    }
                }
                break;
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

    // private void GenerateVapidKey()
    // {
    //     var keys = VapidHelper.GenerateVapidKeys();
    //     Console.WriteLine("Public: " + keys.PublicKey);
    //     Console.WriteLine("Private: " + keys.PrivateKey);

    // }
}
