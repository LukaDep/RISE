using Microsoft.AspNetCore.Components;
using Rise.Shared.Menu;
using Rise.Shared.Common;

namespace Rise.Client.Menu;

public partial class MenuPage : ComponentBase
{
    [Parameter] public string RestoId { get; set; } = default!;

    [Inject] private IMenuService MenuService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    protected List<MenuDto.Index> menus = new();
    protected bool isLoading = true;
    protected HashSet<DateTime> expandedDays = new();
    protected MenuFilter currentFilter = MenuFilter.All;

    protected override async Task OnInitializedAsync()
    {
        await LoadMenusAsync();
    }

    private async Task LoadMenusAsync()
    {
        try
        {
            isLoading = true;

            var result = await MenuService.GetIndexAsync(new QueryRequest.SkipTake
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
            // 🚀 Forceer render-update zodat expandedDays direct zichtbaar wordt
            StateHasChanged();
        }
    }

    private bool IsTodayOrFuture(DayOfWeek menuDay)
    {
        var today = DateTime.Now.DayOfWeek;

        if (today == DayOfWeek.Saturday || today == DayOfWeek.Sunday)
            today = DayOfWeek.Monday;

        return menuDay >= today;
    }

    protected void ToggleDay(DateTime day)
    {
        if (!expandedDays.Add(day))
            expandedDays.Remove(day);
    }

    protected void SetFilter(MenuFilter filter)
    {
        currentFilter = filter;
    }

    protected bool ShouldShowItem(MenuItemDto.Index item) =>
        currentFilter switch
        {
            MenuFilter.Veggie => item.IsVeggie,
            MenuFilter.Vegan => item.IsVegan,
            _ => true
        };

    protected void GoBack()
    {
        NavigationManager.NavigateTo("/resto");
    }

    protected enum MenuFilter
    {
        All,
        Veggie,
        Vegan
    }
}
