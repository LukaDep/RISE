using Microsoft.AspNetCore.Components;

namespace Rise.Client.Account.Notificationsettings;

public partial class Index : ComponentBase
{
    private bool _allNotifications = true;
    private bool allNotifications
    {
        get => _allNotifications;
        set
        {
            _allNotifications = value;
            if (!value)
            {
                gradesNotifications = false;
                scheduleNotifications = false;
                campusNotifications = false;
                newsNotifications = false;
            }
            else
            {
                gradesNotifications = true;
                scheduleNotifications = true;
                campusNotifications = true;
                newsNotifications = true;
            }
        }
    }

    private bool gradesNotifications = true;
    private bool scheduleNotifications = true;
    private bool campusNotifications = true;
    private bool newsNotifications = true;

    private void GoBack()
    {
        NavigationManager.NavigateTo("/account");
    }
}
