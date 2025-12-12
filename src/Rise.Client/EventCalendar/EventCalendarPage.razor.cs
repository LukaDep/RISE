

using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.Events;
using System.Web;

namespace Rise.Client.EventCalendar;

/// <summary>
/// Code-behind for the EventCalendarPage component.
/// Displays a filterable list of events grouped by month.
/// </summary>
public partial class EventCalendarPage : ComponentBase
{
    /// <summary>Service for event data operations.</summary>
    [Inject] public required IEventService EventService { get; set; }
    
    /// <summary>Navigation manager for URL handling.</summary>
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;

    private IEnumerable<EventDTO.Index>? events;
    private List<string>? types = new List<string>();
    private string? activeFilter = null;

    /// <summary>Current filter from query string.</summary>
    public string? Filter { get; set; }

    private int skip = 0;
    private int take = 100;

    /// <summary>
    /// Loads event data when parameters change.
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        EventResponse.Index result = await GetData();
        events = (result.Event ?? Enumerable.Empty<EventDTO.Index>())
         .OrderBy(e => e.StartDateTime)
         .ToList();
        types = result.Event.Select(x => x.Type).Distinct().ToList();
        skip = 0;
    }

    /// <summary>
    /// Retrieves event data from the service with current filters.
    /// </summary>
    /// <returns>The event response containing filtered events.</returns>
    protected async Task<EventResponse.Index> GetData()
    {
        QueryRequest.SkipTake request = new()
        {
            Skip = skip,
            Take = take,
            SearchTerm = "",
            Filters = new Dictionary<string, object?>()
            {
                { "Type", activeFilter ?? "" }
            }
        };

        return await EventService.GetIndexAsync(request);
    }

    /// <summary>
    /// Refreshes the event data asynchronously and updates the UI.
    /// </summary>
    protected async void refreshData()
    {
        EventResponse.Index result = await GetData();
        events = (result.Event ?? Enumerable.Empty<EventDTO.Index>())
         .OrderBy(e => e.StartDateTime)
         .ToList();
        StateHasChanged();
    }

    /// <summary>
    /// Initializes the component and subscribes to location change events.
    /// </summary>
    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += OnLocationChanged;
    }

    /// <summary>
    /// Handles location changes to update the active filter from query parameters.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The location changed event arguments.</param>
    private void OnLocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var currentQueryParams = HttpUtility.ParseQueryString(uri.Query);
        Filter = currentQueryParams.Get("filter") ?? "";
        activeFilter = Filter;
        refreshData();
    }

    /// <summary>
    /// Sets the active filter and refreshes the event data.
    /// </summary>
    /// <param name="filter">The filter type to apply.</param>
    private async void SetFilter(string? filter)
    {
        activeFilter = filter;
        await refreshDataAsync();
    }

    /// <summary>
    /// Refreshes event data asynchronously and updates available filter types.
    /// </summary>
    private async Task refreshDataAsync()
    {
        EventResponse.Index result = await GetData();
        events = (result.Event ?? Enumerable.Empty<EventDTO.Index>())
         .OrderBy(e => e.StartDateTime)
         .ToList();
        types = result.Event.Select(x => x.Type).Distinct().ToList();
        StateHasChanged();
    }

    /// <summary>
    /// Gets the CSS class for a filter button based on its active state.
    /// </summary>
    /// <param name="filter">The filter value to check.</param>
    /// <returns>CSS classes for the button styling.</returns>
    private string GetFilterButtonClass(string? filter)
    {
        var isActive = activeFilter == filter;
        return isActive
            ? "px-3 py-1.5 text-sm font-medium rounded-lg bg-hogent-education text-white shadow-sm transition-all"
            : "px-3 py-1.5 text-sm font-medium rounded-lg bg-white text-gray-700 border border-gray-200 hover:bg-gray-50 hover:border-gray-300 transition-all";
    }

    /// <summary>
    /// Gets the CSS class for a filter chip based on its active state.
    /// </summary>
    /// <param name="type">The event type to check.</param>
    /// <returns>CSS classes for the chip styling.</returns>
    private string GetFilterChipClass(string type)
    {
        var isActive = activeFilter == type;
        return isActive
            ? "inline-flex items-center gap-1.5 px-3 py-1.5 text-sm font-medium rounded-full bg-hogent-education text-white shadow-sm transition-all"
            : "inline-flex items-center gap-1.5 px-3 py-1.5 text-sm font-medium rounded-full bg-white text-gray-600 border border-gray-200 hover:bg-gray-50 hover:border-gray-300 transition-all cursor-pointer";
    }

    /// <summary>
    /// Gets the background color class for an event type.
    /// </summary>
    /// <param name="type">The event type.</param>
    /// <returns>CSS background color class.</returns>
    private static string GetEventTypeColor(string type) => type.ToLower() switch
    {
        "welzijn" => "bg-hogent-education",
        "andere" => "bg-hogent-it",
        "sport" => "bg-green-500",
        "cultuur" => "bg-purple-500",
        "feest" => "bg-pink-500",
        "academisch" => "bg-blue-500",
        _ => "bg-hogent-black-50"
    };

    /// <summary>
    /// Gets the badge CSS classes for an event type.
    /// </summary>
    /// <param name="type">The event type.</param>
    /// <returns>CSS classes for badge background and text color.</returns>
    private static string GetEventTypeBadgeClass(string type) => type.ToLower() switch
    {
        "welzijn" => "bg-hogent-education-15 text-hogent-education",
        "andere" => "bg-hogent-it-15 text-hogent-it",
        "sport" => "bg-green-100 text-green-700",
        "cultuur" => "bg-purple-100 text-purple-700",
        "feest" => "bg-pink-100 text-pink-700",
        "academisch" => "bg-blue-100 text-blue-700",
        _ => "bg-gray-100 text-gray-700"
    };
}