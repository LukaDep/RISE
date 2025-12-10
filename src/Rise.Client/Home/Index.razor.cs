using Microsoft.AspNetCore.Components;
using Rise.Shared.Widgets;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using Rise.Shared.Schedule;
using Rise.Shared.Resto;
using Rise.Shared.Common;

namespace Rise.Client.Home;

public record WidgetEntry(Type Type, Guid Id, int Width, int Height, int X, int Y, int MinWidth);

public partial class Index : ComponentBase
{
    [Inject] public IJSRuntime Js { get; set; } = default!;
    [Inject] public IWidgetService WidgetService { get; set; } = default!;
    [Inject] public IScheduleService ScheduleClientService { get; set; } = default!;
    [Inject] public IRestoService RestoClientService { get; set; } = default!;
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;
    [Parameter] public DateTime? StartDate { get; set; }
    [Parameter] public DateTime? EndDate { get; set; }
    private List<ScheduleDto.Schedule>? UpcomingClasses { get; set; }
    [Parameter] public string? Mode { get; set; }
    private bool _isAddOpen;

    [CascadingParameter]
    private Task<AuthenticationState>? AuthState { get; set; }

    private void ToggleAdd()
    {
        _isAddOpen = !_isAddOpen;
    }

    private void CloseAdd(FocusEventArgs? _ = null)
    {
        _isAddOpen = false;
        StateHasChanged();
    }

    private static readonly List<Type> AllWidgets = new()
    {
        typeof(Widgets.ScheduleWidget),
        typeof(Widgets.RestoMenuWidget),
        typeof(Widgets.NewsWidget),
        typeof(Widgets.GradesWidget),
        typeof(Widgets.LinksWidget)
    };

    // Add new choices for widgets here
    private readonly Dictionary<string, Type> _map = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
    {
        ["Schedule"] = typeof(Widgets.ScheduleWidget),
        ["Menus"] = typeof(Widgets.RestoMenuWidget),
        ["News"] = typeof(Widgets.NewsWidget),
        ["Grades"] = typeof(Widgets.GradesWidget),
        ["Links"] = typeof(Widgets.LinksWidget)
    };

    private static WidgetEntry CreateDefault(Type t, int height, int y) =>
        new WidgetEntry(t, Guid.NewGuid(), 12, height, 0, y, 4);

    private static readonly List<WidgetEntry> DefaultAnonymous = new()
    {
        CreateDefault(typeof(Widgets.LinksWidget),5, 0),
        CreateDefault(typeof(Widgets.RestoMenuWidget),8, 6),
        CreateDefault(typeof(Widgets.NewsWidget),5, 14)
    };

    private static readonly List<WidgetEntry> DefaultLoggedIn = new()
    {
        CreateDefault(typeof(Widgets.LinksWidget),5, 0),
        CreateDefault(typeof(Widgets.ScheduleWidget),6, 6),
        CreateDefault(typeof(Widgets.NewsWidget),5, 13),
        CreateDefault(typeof(Widgets.GradesWidget),4, 19),
        CreateDefault(typeof(Widgets.RestoMenuWidget),8, 24)
    };

    private List<WidgetEntry> _currentWidgets = new(); private IEnumerable<Type> AvailableWidgets
        => AllWidgets.Except(_currentWidgets.Select(w => w.Type));


    private bool IsEditMode => string.Equals(Mode, "edit", StringComparison.OrdinalIgnoreCase);

    private void EnterEdit()
    {
        NavigationManager.NavigateTo("/edit");
    }

    private async Task SaveAndExit()
    {
        var updatedWidgets = await GetCurrentWidgetLayoutAsync();

        // Update the CurrentWidgets list with the new positions and sizes
        _currentWidgets = _currentWidgets.Select(widget =>
        {
            var updatedWidget = updatedWidgets.FirstOrDefault(w => w.Id == widget.Id);
            return updatedWidget != null
                ? widget with { X = updatedWidget.X, Y = updatedWidget.Y, Width = updatedWidget.Width, Height = updatedWidget.Height }
                : widget;
        }).ToList();

        // Prepare the update request
        var updateRequest = new WidgetRequest.Update
        {
            UserWidgets = _currentWidgets.Select(w =>
             {
                 var mapEntry = _map.FirstOrDefault(x => x.Value == w.Type);
                 return new UserWidgetDto.Update
                 {
                     Id = w.Id,
                     WidgetName = mapEntry.Key?.ToLowerInvariant() ?? string.Empty,
                     X = w.X,
                     Y = w.Y,
                     Width = w.Width,
                     Height = w.Height,
                     MinWidth = w.MinWidth
                 };
             })
        };
        try { await WidgetService.UpdateUserWidgetsAsync(updateRequest); }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
        }
        NavigationManager.NavigateTo("/");
    }

    private void AddWidget(Type widgetType)
    {

        if (_currentWidgets.Any(w => w.Type == widgetType)) return;

        var nextY = _currentWidgets.Any()
            ? _currentWidgets.Max(w => w.Y + w.Height)
            : 0;
        var newWidget = new WidgetEntry(
            widgetType,
            Guid.NewGuid(),
            Width: 12, // Default width
            Height: 6, // Default height
            X: 0,      // Default X position
            Y: nextY,  // Calculated Y position
            MinWidth: 4 // Default minimum width
        );
        _currentWidgets.Add(newWidget);
        _isAddOpen = false;
        StateHasChanged();
    }
    private Task RemoveItem(Guid index)
    {
        var idx = _currentWidgets.FindIndex(w => w.Id == index);
        if (idx >= 0)
        {
            _currentWidgets.RemoveAt(idx);
            StateHasChanged();
        }
        return Task.CompletedTask;
    }

    protected override async Task OnInitializedAsync()
    {
        QueryRequest.DateRange request = new()
        {
            Skip = 0,
            Take = 200,
            StartDate = StartDate,
            EndDate = EndDate
        };

        var resultClasses = await ScheduleClientService.GetIndexAsync(request);
        UpcomingClasses = resultClasses
            .Value?
            .Schedules
            .Where(r => r.StartDateTime.Date == DateTime.Today.Date)
            .OrderBy(r => r.StartDateTime)
            .ToList();

        var resultRestos = await RestoClientService.GetIndexAsync(new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 200
        });

        try
        {
            var isAuthenticated = AuthState?.Result.User?.Identity?.IsAuthenticated == true;

            var res = await WidgetService.GetIndexByUserIdAsync();

            if (res.IsSuccess && res.Value?.UserWidgets != null && res.Value.UserWidgets.Any())
            {


                var ordered = res.Value.UserWidgets
                    .OrderBy(uw => uw.Y)
                    .ThenBy(uw => uw.X);

                var list = ordered
                    .Where(uw => uw.Widget?.Key != null && _map.ContainsKey(uw.Widget.Key))
                    .Select(uw =>
                    {
                        var key = uw.Widget!.Key!;
                        var type = _map[key];
                        return new WidgetEntry(type, uw.Id, uw.Width, uw.Height, uw.X, uw.Y, uw.MinWidth);
                    })
                    .ToList();

                _currentWidgets = list;
            }
            else
            {
                // no widgets from server -> choose default based on authentication
                _currentWidgets = isAuthenticated
                    ? DefaultLoggedIn.Select(w => new WidgetEntry(w.Type, Guid.NewGuid(), w.Width, w.Height, w.X, w.Y, w.MinWidth)).ToList()
                    : DefaultAnonymous.Select(w => new WidgetEntry(w.Type, Guid.NewGuid(), w.Width, w.Height, w.X, w.Y, w.MinWidth)).ToList();
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            _currentWidgets = DefaultAnonymous.Select(w => new WidgetEntry(w.Type, Guid.NewGuid(), w.Width, w.Height, w.X, w.Y, w.MinWidth)).ToList();
        }
    }

    private async Task<IEnumerable<WidgetEntry>> GetCurrentWidgetLayoutAsync()
    {
        var widgets = await Js.InvokeAsync<List<WidgetEntry>>("gridstackInterop.getWidgets");
        return widgets;
    }


}