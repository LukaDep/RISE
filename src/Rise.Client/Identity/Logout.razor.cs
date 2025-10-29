using Microsoft.AspNetCore.Components;

namespace Rise.Client.Identity;

public partial class Logout
{
    [Inject] public required IAccountManager AccountManager { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (await AccountManager.CheckAuthenticatedAsync())
        {
            await AccountManager.LogoutAsync();
        }
        NavigationManager.NavigateTo("/login", true);
    }
}