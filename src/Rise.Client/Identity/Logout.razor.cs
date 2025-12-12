using Microsoft.AspNetCore.Components;

namespace Rise.Client.Identity;

/// <summary>
/// Code-behind for the Logout page component.
/// Handles user logout and redirection to home.
/// </summary>
public partial class Logout
{
    /// <summary>Account manager for authentication operations.</summary>
    [Inject] public required IAccountManager AccountManager { get; set; }
    
    /// <summary>Navigation manager for URL handling.</summary>
    [Inject] public required NavigationManager NavigationManager { get; set; }

    /// <summary>
    /// Logs out the user and redirects to home page.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        if (await AccountManager.CheckAuthenticatedAsync())
        {
            await AccountManager.LogoutAsync();
        }
        NavigationManager.NavigateTo("/", true);
    }
}