using Microsoft.AspNetCore.Components;
using Rise.Shared.Menu;
using Rise.Client.Menu;
using Rise.Shared.Common;

public class WeekMenuBase : ComponentBase
{
    [Inject] public IMenuService MenuService { get; set; } = default!;

    protected bool IsLoading { get; set; } = true;
    protected List<MenuDto.Index> Menus { get; set; } = new();
    protected List<MenuDto.Index> FilteredMenus { get; set; } = new();

    protected string ActiveFilter { get; set; } = "All";

    protected override async Task OnInitializedAsync()
    {
        Console.WriteLine("test");
        try
        {
            Console.WriteLine("📡 Fetching weekly menus...");
            var result = await MenuService.GetIndexAsync(new QueryRequest.SkipTake { Skip = 0, Take = 7 });

            if (result is { IsSuccess: true, Value.Menus: not null })
            {
                Menus = result.Value.Menus.ToList();
                Console.WriteLine($"✅ Menus fetched: {Menus.Count}");
                FilterMenus();
            }
            else
            {
                Console.WriteLine("❌ Failed to fetch menus or no data returned.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔥 Error while fetching menus: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    protected void SetFilter(string filter)
    {
        ActiveFilter = filter;
        Console.WriteLine($"🔸 Active filter set to: {ActiveFilter}");
        FilterMenus();
    }

    private void FilterMenus()
    {
        if (Menus == null || Menus.Count == 0)
        {
            FilteredMenus = new();
            Console.WriteLine("⚠️ No menus available to filter.");
            StateHasChanged();
            return;
        }

        if (ActiveFilter == "All")
        {
            FilteredMenus = Menus.ToList();
        }
        else if (ActiveFilter == "Veggie")
        {
            FilteredMenus = Menus
                .Select(m => new MenuDto.Index
                {
                    Id = m.Id,
                    Date = m.Date,
                    MenuItems = m.MenuItems.Where(i => i.IsVeggie).ToList()
                })
                .Where(m => m.MenuItems.Any())
                .ToList();
        }
        else if (ActiveFilter == "Vegan")
        {
            FilteredMenus = Menus
                .Select(m => new MenuDto.Index
                {
                    Id = m.Id,
                    Date = m.Date,
                    MenuItems = m.MenuItems.Where(i => i.IsVegan).ToList()
                })
                .Where(m => m.MenuItems.Any())
                .ToList();
        }

        Console.WriteLine($"📊 FilteredMenus count = {FilteredMenus.Count}");
        StateHasChanged();
    }
}
