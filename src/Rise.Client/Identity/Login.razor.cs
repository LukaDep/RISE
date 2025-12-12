using Microsoft.AspNetCore.Components;
using Rise.Shared.Identity.Accounts;
using Rise.Shared.Notifications;

namespace Rise.Client.Identity;

/// <summary>
/// Code-behind for the Login page component.
/// Handles user authentication and push notification synchronization.
/// </summary>
public partial class Login
{
    private AccountRequest.Login Model = new();
    private Result _result = new();
    
    /// <summary>Account manager for authentication operations.</summary>
    [Inject] public required IAccountManager AccountManager { get; set; }
    
    /// <summary>Navigation manager for URL handling.</summary>
    [Inject] public required NavigationManager Navigation { get; set; }
    
    /// <summary>Service for notification preferences.</summary>
    [Inject] public required INotificationPreferencesService NotificationPreferencesService { get; set; }

    /// <summary>
    /// Attempts to log in the user with the provided credentials.
    /// </summary>
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