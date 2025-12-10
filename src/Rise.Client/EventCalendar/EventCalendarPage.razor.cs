

using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.Events;
using System.Web;

namespace Rise.Client.EventCalendar;

public partial class EventCalendarPage : ComponentBase
{
    [Inject] public required IEventService EventService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;

    private IEnumerable<EventDTO.Index>? events;
    private List<string>? types = new List<string>();
    private string? activeFilter = null;

    public string? Filter { get; set; }

    private int skip = 0;
    private int take = 100;

    protected override async Task OnParametersSetAsync()
    {
        EventResponse.Index result = await GetData();
        events = (result.Event ?? Enumerable.Empty<EventDTO.Index>())
         .OrderBy(e => e.StartDateTime)
         .ToList();
        types = result.Event.Select(x => x.Type).Distinct().ToList();
        skip = 0;
    }

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

    protected async void refreshData()
    {
        EventResponse.Index result = await GetData();
        events = (result.Event ?? Enumerable.Empty<EventDTO.Index>())
         .OrderBy(e => e.StartDateTime)
         .ToList();
        StateHasChanged();
    }

    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += OnLocationChanged;
    }

    private void OnLocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var currentQueryParams = HttpUtility.ParseQueryString(uri.Query);
        Filter = currentQueryParams.Get("filter") ?? "";
        activeFilter = Filter;
        refreshData();
    }

    private async void SetFilter(string? filter)
    {
        activeFilter = filter;
        await refreshDataAsync();
    }

    private async Task refreshDataAsync()
    {
        EventResponse.Index result = await GetData();
        events = (result.Event ?? Enumerable.Empty<EventDTO.Index>())
         .OrderBy(e => e.StartDateTime)
         .ToList();
        types = result.Event.Select(x => x.Type).Distinct().ToList();
        StateHasChanged();
    }

    private string GetFilterButtonClass(string? filter)
    {
        var isActive = activeFilter == filter;
        return isActive
            ? "px-3 py-1.5 text-sm font-medium rounded-lg bg-hogent-education text-white shadow-sm transition-all"
            : "px-3 py-1.5 text-sm font-medium rounded-lg bg-white text-gray-700 border border-gray-200 hover:bg-gray-50 hover:border-gray-300 transition-all";
    }

    private string GetFilterChipClass(string type)
    {
        var isActive = activeFilter == type;
        return isActive
            ? "inline-flex items-center gap-1.5 px-3 py-1.5 text-sm font-medium rounded-full bg-hogent-education text-white shadow-sm transition-all"
            : "inline-flex items-center gap-1.5 px-3 py-1.5 text-sm font-medium rounded-full bg-white text-gray-600 border border-gray-200 hover:bg-gray-50 hover:border-gray-300 transition-all cursor-pointer";
    }

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