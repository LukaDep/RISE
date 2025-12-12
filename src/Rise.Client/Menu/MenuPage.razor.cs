using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.Menu;

namespace Rise.Client.Menu;

/// <summary>
/// Code-behind for the MenuPage component.
/// Displays the weekly menu for a specific restaurant.
/// </summary>
public partial class MenuPage : ComponentBase
{
    /// <summary>The ID of the restaurant to display menus for.</summary>
    [Parameter] public Guid RestoId { get; set; } = default!;

    /// <summary>Service for menu data operations.</summary>
    [Inject] private IMenuService MenuService { get; set; } = default!;
    
    /// <summary>Navigation manager for URL handling.</summary>
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    /// <summary>The list of menus to display.</summary>
    protected List<MenuDto.Index> menus = new();
    
    /// <summary>Indicates whether data is being loaded.</summary>
    protected bool isLoading = true;
    
    /// <summary>Set of days that are currently expanded.</summary>
    protected HashSet<DateTime> expandedDays = new();
    
    /// <summary>The current menu filter (All, Veggie, Vegan).</summary>
    protected MenuFilter currentFilter = MenuFilter.All;

    /// <summary>
    /// Initializes the component and loads menu data.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        await LoadMenusAsync();
    }

    /// <summary>
    /// Loads menu data from the service for the specified restaurant.
    /// </summary>
    private async Task LoadMenusAsync()
    {
        Console.WriteLine(RestoId);
        try
        {
            isLoading = true;

            var result = await MenuService.GetIndexAsync(new QueryRequest.DateRange()
            {
                Skip = 0,
                Take = 100
            });

            if (result.IsSuccess)
            {
                menus = result.Value.Menus
                    .Where(m => m.RestoId == RestoId)
                    // Enkel werkdagen (ma–vr)
                    .Where(m => m.Date.DayOfWeek is >= DayOfWeek.Monday and <= DayOfWeek.Friday)
                    // Geen verleden, enkel vandaag en toekomst
                    .Where(m => IsTodayOrFuture(m.Date.DayOfWeek))
                    .OrderBy(m => m.Date)
                    .ToList();

                // ✅ Huidige dag automatisch openklappen
                var today = DateTime.Now.DayOfWeek;

                // Als het weekend is, toon maandag
                if (today == DayOfWeek.Saturday || today == DayOfWeek.Sunday)
                    today = DayOfWeek.Monday;

                var todayMenu = menus.FirstOrDefault(m => m.Date.DayOfWeek == today);
                if (todayMenu != null)
                {
                    expandedDays.Clear();
                    expandedDays.Add(todayMenu.Date.Date);
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[MenuPage] Error loading menus: {ex.Message}");
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Checks if a menu day is today or in the future.
    /// </summary>
    /// <param name="menuDay">The day of week to check.</param>
    /// <returns>True if the day is today or later in the week.</returns>
    private bool IsTodayOrFuture(DayOfWeek menuDay)
    {
        var today = DateTime.Now.DayOfWeek;

        if (today == DayOfWeek.Saturday || today == DayOfWeek.Sunday)
            today = DayOfWeek.Monday;

        return menuDay >= today;
    }

    /// <summary>
    /// Toggles the expanded state of a day's menu section.
    /// </summary>
    /// <param name="day">The day to toggle.</param>
    protected void ToggleDay(DateTime day)
    {
        if (!expandedDays.Add(day))
            expandedDays.Remove(day);
    }

    /// <summary>
    /// Sets the current menu filter.
    /// </summary>
    /// <param name="filter">The filter to apply.</param>
    protected void SetFilter(MenuFilter filter)
    {
        currentFilter = filter;
    }

    /// <summary>
    /// Determines if a menu item should be displayed based on the current filter.
    /// </summary>
    /// <param name="item">The menu item to check.</param>
    /// <returns>True if the item matches the current filter.</returns>
    protected bool ShouldShowItem(MenuItemDto.Index item) =>
        currentFilter switch
        {
            MenuFilter.Veggie => item.IsVeggie,
            MenuFilter.Vegan => item.IsVegan,
            _ => true
        };

    /// <summary>
    /// Menu filter options for dietary preferences.
    /// </summary>
    protected enum MenuFilter
    {
        All,
        Veggie,
        Vegan
    }
}
