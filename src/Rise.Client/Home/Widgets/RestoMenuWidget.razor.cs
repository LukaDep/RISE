using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rise.Shared.Common;
using Rise.Shared.Menu;
using Rise.Shared.Resto;

namespace Rise.Client.Home.Widgets;

public partial class RestoMenuWidget : ComponentBase
{
    [Parameter] public Guid? SelectedResto { get; set; }
    [Parameter] public EventCallback<Guid?> SelectedRestoChanged { get; set; }
    [Parameter] public EventCallback<Guid> OnRemove { get; set; }
    [Parameter] public bool EditMode { get; set; }
    [Parameter] public int Index { get; set; }
    [Parameter] public Guid WidgetId { get; set; }
    [Inject] public IJSRuntime Js { get; set; } = default!;

    private MenuDto.Index? _currentMenu;
    private List<RestoDto.Index> _restoList = new();
    private int _currentIndex = 0;
    private bool _canNavigate => _restoList.Count > 1;

    // Fast lookups
    private IImmutableDictionary<Guid, MenuDto.Index> _menuByRestoId = ImmutableDictionary<Guid, MenuDto.Index>.Empty;

    private RestoDto.Index? CurrentResto =>
        (_restoList.Count > 0 && _currentIndex >= 0 && _currentIndex < _restoList.Count)
            ? _restoList[_currentIndex]
            : null;

    private bool _loading;
    private string? _error;

    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        try
        {
            // fetch restos
            var restosResult = await RestoClientService.GetIndexAsync(new QueryRequest.SkipTake
            {
                Skip = 0,
                Take = 50
            });
            _restoList = (restosResult.Value?.Restos ?? []).ToList();

            // fetch menus and filter for *local* today (handles UTC vs local storage)
            var menusResult = await MenuClientService.GetIndexAsync(new QueryRequest.SkipTake()
            {
                Skip = 0,
                Take = 200,
            });
            var todays = (menusResult.Value?.Menus ?? []).Where(m => m.Date.DayOfWeek.Equals(DateTime.Today.DayOfWeek)).ToList();

            // var todays = menusResult.Value?.Menus
            //     .Where(m => m.Date.ToLocalTime().Date == DateTime.Today)
            //     .ToList();



            _menuByRestoId = todays
                .GroupBy(m => m.RestoId)
                .ToImmutableDictionary(g => g.Key, g => g.First());

            // select initial resto
            if (SelectedResto.HasValue)
            {
                var idx = _restoList.FindIndex(r => r.Id == SelectedResto.Value);
                _currentIndex = idx >= 0 ? idx : 0;
            }
            else
            {
                _currentIndex = 0;
                if (_restoList.Any())
                {
                    SelectedResto = _restoList[_currentIndex].Id;
                }
            }

            SetCurrentMenu();
        }
        catch (Exception ex)
        {
            _error = "Failed to load menus or restos";
            Console.Error.WriteLine(ex);
        }
        finally
        {
            _loading = false;
            StateHasChanged();
        }
    }

    private void SetCurrentMenu()
    {
        _currentMenu = null;

        // prefer explicit selection, otherwise use the currently displayed resto id
        var restoId = SelectedResto ?? CurrentResto?.Id;

        if (restoId.HasValue && _menuByRestoId.TryGetValue(restoId.Value, out var menu))
        {
            _currentMenu = menu;
            return;
        }

        // fallback to any available menu for today
        _currentMenu = _menuByRestoId.Values.FirstOrDefault();
    }

    private async Task Prev()
    {
        if (!_canNavigate) return;
        _currentIndex = (_currentIndex - 1 + _restoList.Count) % _restoList.Count;
        SelectedResto = CurrentResto?.Id;
        if (SelectedRestoChanged.HasDelegate)
            await SelectedRestoChanged.InvokeAsync(SelectedResto);
        SetCurrentMenu();
    }

    private async Task Next()
    {
        if (!_canNavigate) return;
        _currentIndex = (_currentIndex + 1) % _restoList.Count;
        SelectedResto = CurrentResto?.Id;
        if (SelectedRestoChanged.HasDelegate)
            await SelectedRestoChanged.InvokeAsync(SelectedResto);
        SetCurrentMenu();
    }

    private void More()
    {
        NavigationManager.NavigateTo("/resto");
    }
}