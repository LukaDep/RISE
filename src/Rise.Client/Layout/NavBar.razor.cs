namespace Rise.Client.Layout;

using Microsoft.AspNetCore.Components;

/// <summary>
/// Code-behind for the NavBar layout component.
/// Provides bottom navigation with dynamic route highlighting.
/// </summary>
public partial class NavBar : ComponentBase, IDisposable
{
    /// <summary>Navigation manager for route handling.</summary>
    [Inject] NavigationManager NavigationManager { get; set; } = default!;

    private bool _showMore;
    private bool _moreRouteActive;
    private bool _homeRouteActive;
    
    /// <summary>Routes shown in the "more" dropdown menu.</summary>
    private readonly string[] _moreRoutes = new[]
    {
        "resto",
        "grades",
        "news",
        "events",
        "campus",
        "contact",
        "account/notifications",
        "deadlines"
    };

    /// <summary>Toggles the more dropdown visibility.</summary>
    void ToggleMore() => _showMore = !_showMore;
    
    /// <summary>Closes the more dropdown.</summary>
    void CloseMore() => _showMore = false;

    /// <summary>
    /// Initializes the component and subscribes to location changes.
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        UpdateMoreRouteActive();
        UpdateHomeRouteActive();
        NavigationManager.LocationChanged += OnLocationChanged;
    }

    /// <summary>
    /// Handles location changes to update active route highlighting.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The location changed event arguments.</param>
    private void OnLocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
    {
        UpdateMoreRouteActive();
        UpdateHomeRouteActive();
        InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Updates the more routes active state based on current URL.
    /// </summary>
    private void UpdateMoreRouteActive()
    {
        var relative = NavigationManager.ToBaseRelativePath(NavigationManager.Uri).TrimStart('/');
        _moreRouteActive = _moreRoutes.Any(r => relative.Equals(r, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Updates the home route active state based on current URL.
    /// </summary>
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