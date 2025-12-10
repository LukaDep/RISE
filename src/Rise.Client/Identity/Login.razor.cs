using Microsoft.AspNetCore.Components;
using Rise.Shared.Identity.Accounts;
using Rise.Shared.Notifications;

namespace Rise.Client.Identity;

public partial class Login
{
    private AccountRequest.Login Model = new();
    private Result _result = new();
    [Inject] public required IAccountManager AccountManager { get; set; }
    [Inject] public required NavigationManager Navigation { get; set; }
    [Inject] public required INotificationPreferencesService NotificationPreferencesService { get; set; }

    public async Task LoginUser()
    {
        _result = await AccountManager.LoginAsync(Model.Email!, Model.Password!);

        if (_result.IsSuccess)
        {
            // Synchronize push notification subscription after login
            // This ensures the server has the correct subscription for this user
            // without asking for permission again
            await NotificationPreferencesService.SyncSubscriptionAsync();

            Navigation.NavigateTo("/");
        }
    }
}