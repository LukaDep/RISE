using Microsoft.AspNetCore.Components;
using Rise.Shared.Menu;
using Rise.Shared.Common;

namespace Rise.Client.Menu;

public partial class MenuPage : ComponentBase
{
    [Parameter] public string RestoId { get; set; } = default!;
    [Inject] private IMenuService MenuService { get; set; } = default!;

    protected List<MenuDto.Index> menus = new();
    protected bool isLoading = true;
    protected HashSet<DateTime> expandedDays = new();

    protected MenuFilter currentFilter = MenuFilter.All;

    protected override async Task OnInitializedAsync()
    {
        try
        {
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
        finally
        {
            isLoading = false;
        }
    }

    protected void ToggleDay(DateTime day)
    {
        if (expandedDays.Contains(day))
            expandedDays.Remove(day);
        else
            expandedDays.Add(day);
    }

    protected void SetFilter(MenuFilter filter)
    {
        currentFilter = filter;
    }

    protected bool ShouldShowItem(MenuItemDto.Index item)
    {
        return currentFilter switch
        {
            MenuFilter.Veggie => item.IsVeggie,
            MenuFilter.Vegan => item.IsVegan,
            _ => true
        };
    }

    protected enum MenuFilter
    {
        All,
        Veggie,
        Vegan
    }
}
