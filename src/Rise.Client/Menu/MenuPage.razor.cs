using Microsoft.AspNetCore.Components;
using Rise.Shared.Menu;
using Rise.Shared.Common;

namespace Rise.Client.Menu;

public partial class MenuPage : ComponentBase
{
    [Parameter] public Guid RestoId { get; set; } = default!;

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
        Console.WriteLine(RestoId);
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
                    .OrderBy(m => m.Date)
                    .ToList();
            }
        }
        catch (Exception ex)
        {
            // eventueel logging toevoegen
            Console.Error.WriteLine($"[MenuPage] Error loading menus: {ex.Message}");
        }
        finally
        {
            isLoading = false;
        }
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
