namespace Rise.Client.Layout;

using Microsoft.AspNetCore.Components;
public partial class NavBar : ComponentBase, IDisposable
{

    [Inject] NavigationManager NavigationManager { get; set; } = default!;

    private bool _showMore;
    private bool _moreRouteActive;
    private bool _homeRouteActive;
    private readonly string[] _moreRoutes = new[]
    {
        "resto",
        "grades",
        "news",
        "events",
        "campus",
        "contact",
        "account/notificationsettings"
    };

    void ToggleMore() => _showMore = !_showMore;
    void CloseMore() => _showMore = false;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        UpdateMoreRouteActive();
        UpdateHomeRouteActive();
        NavigationManager.LocationChanged += OnLocationChanged;
    }

    private void OnLocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
    {
        UpdateMoreRouteActive();
        UpdateHomeRouteActive();
        InvokeAsync(StateHasChanged);
    }

    private void UpdateMoreRouteActive()
    {
        var relative = NavigationManager.ToBaseRelativePath(NavigationManager.Uri).TrimStart('/');
        _moreRouteActive = _moreRoutes.Any(r => relative.Equals(r, StringComparison.OrdinalIgnoreCase));
    }

    private void UpdateHomeRouteActive()
    {
        var relative = NavigationManager.ToBaseRelativePath(NavigationManager.Uri).TrimStart('/');
        _homeRouteActive = string.Empty.Equals(relative, StringComparison.OrdinalIgnoreCase);
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
    }
}