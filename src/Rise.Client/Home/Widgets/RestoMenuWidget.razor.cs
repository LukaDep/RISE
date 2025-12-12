namespace Rise.Client.Home.Widgets;

using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rise.Shared.Common;
using Rise.Shared.Menu;
using Rise.Shared.Resto;

/// <summary>
/// Dashboard widget that displays today's restaurant menu.
/// Supports switching between multiple restaurants.
/// </summary>
public partial class RestoMenuWidget : ComponentBase
{
    /// <summary>Currently selected restaurant ID.</summary>
    [Parameter] public Guid? SelectedResto { get; set; }
    
    /// <summary>Callback when selected restaurant changes.</summary>
    [Parameter] public EventCallback<Guid?> SelectedRestoChanged { get; set; }
    
    /// <summary>Callback when widget is removed.</summary>
    [Parameter] public EventCallback<Guid> OnRemove { get; set; }
    
    /// <summary>Indicates if edit mode is active.</summary>
    [Parameter] public bool EditMode { get; set; }
    
    /// <summary>Widget index in the grid.</summary>
    [Parameter] public int Index { get; set; }
    
    /// <summary>Unique widget identifier.</summary>
    [Parameter] public Guid WidgetId { get; set; }
    
    /// <summary>JavaScript runtime for interop.</summary>
    [Inject] public IJSRuntime Js { get; set; } = default!;
    
    /// <summary>Navigation manager for routing.</summary>
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;
    
    /// <summary>Service for menu data.</summary>
    [Inject] public IMenuService MenuClientService { get; set; } = default!;
    
    /// <summary>Service for restaurant data.</summary>
    [Inject]
    public IRestoService RestoClientService { get; set; } = default!;
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

    /// <summary>
    /// Initializes the widget by loading restaurant and menu data.
    /// Fetches all restaurants and today's menus, builds a lookup dictionary
    /// for fast menu retrieval by restaurant ID, and sets the initial selection.
    /// If a SelectedResto is provided, navigates to that restaurant's index;
    /// otherwise defaults to the first restaurant in the list.
    /// </summary>
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

    /// <summary>
    /// Updates the current menu based on selected or displayed restaurant.
    /// First tries to find a menu for the explicitly selected restaurant,
    /// then falls back to the currently displayed restaurant's menu,
    /// and finally defaults to any available menu for today.
    /// </summary>
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

    /// <summary>
    /// Navigates to the previous restaurant in the carousel.
    /// Uses modulo arithmetic to wrap around to the last restaurant when at the first.
    /// Updates the selected restaurant and notifies parent via callback.
    /// </summary>
    private async Task Prev()
    {
        if (!_canNavigate) return;
        _currentIndex = (_currentIndex - 1 + _restoList.Count) % _restoList.Count;
        SelectedResto = CurrentResto?.Id;
        if (SelectedRestoChanged.HasDelegate)
            await SelectedRestoChanged.InvokeAsync(SelectedResto);
        SetCurrentMenu();
    }

    /// <summary>
    /// Navigates to the next restaurant in the carousel.
    /// Uses modulo arithmetic to wrap around to the first restaurant when at the last.
    /// Updates the selected restaurant and notifies parent via callback.
    /// </summary>
    private async Task Next()
    {
        if (!_canNavigate) return;
        _currentIndex = (_currentIndex + 1) % _restoList.Count;
        SelectedResto = CurrentResto?.Id;
        if (SelectedRestoChanged.HasDelegate)
            await SelectedRestoChanged.InvokeAsync(SelectedResto);
        SetCurrentMenu();
    }

    /// <summary>
    /// Navigates to the full restaurant list page.
    /// </summary>
    private void More()
    {
        NavigationManager.NavigateTo("/resto");
    }
}