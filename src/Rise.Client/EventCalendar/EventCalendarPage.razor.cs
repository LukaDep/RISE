

using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.Events;
using System.Web;

namespace Rise.Client.EventCalendar;

public partial class EventCalendarPage : ComponentBase
{
    [Inject] public required IEventService EventService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;

    private EventDTO.Index? SelectedEvent = null;
    private IEnumerable<EventDTO.Index>? events;
    private List<string>? types = new List<string>();

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
                { "Type", Filter ?? "" }
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
        refreshData();
    }

    private void ShowDetails(EventDTO.Index specificEvent)
    {
        SelectedEvent = specificEvent;
    }

    private void CloseDetails()
    {
        SelectedEvent = null;
    }
}