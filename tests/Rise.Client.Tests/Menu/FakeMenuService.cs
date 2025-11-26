using Ardalis.Result;
using Rise.Shared.Common;
using Rise.Shared.Menu;

namespace Rise.Client.Menu;

public class FakeMenuService : IMenuService
{
    private static readonly Guid Resto1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid Resto2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public Task<Result<MenuResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ct = default)
    {
        var today = DateTime.Today;
        
        // Find next Monday (or today if it's Monday)
        var daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
        var monday = daysUntilMonday == 0 ? today : today.AddDays(daysUntilMonday);
        
        // If today is a weekday, use today; otherwise use next Monday
        var firstDay = today.DayOfWeek >= DayOfWeek.Monday && today.DayOfWeek <= DayOfWeek.Friday ? today : monday;

        var menu1Id = Guid.NewGuid();
        var menu2Id = Guid.NewGuid();
        var menu3Id = Guid.NewGuid();

        var menus = new List<MenuDto.Index>
        {
            new MenuDto.Index
            {
                Id = menu1Id,
                RestoId = Resto1Id,
                Date = firstDay,
                MenuItems = new List<MenuItemDto.Index>
                {
                    new MenuItemDto.Index
                    {
                        Id = Guid.NewGuid(),
                        MenuId = menu1Id,
                        Name = "Spaghetti Bolognese",
                        Description = "Classic Italian pasta with meat sauce",
                        PriceStudent = 4.50,
                        PriceExtern = 6.50,
                        Type = FoodType.WarmeMaaltijd,
                        IsVegan = false,
                        IsVeggie = false
                    },
                    new MenuItemDto.Index
                    {
                        Id = Guid.NewGuid(),
                        MenuId = menu1Id,
                        Name = "Vegetarian Lasagna",
                        Description = "Layered pasta with vegetables",
                        PriceStudent = 4.20,
                        PriceExtern = 6.20,
                        Type = FoodType.WarmeMaaltijd,
                        IsVegan = false,
                        IsVeggie = true
                    },
                    new MenuItemDto.Index
                    {
                        Id = Guid.NewGuid(),
                        MenuId = menu1Id,
                        Name = "Tomato Soup",
                        Description = "Fresh tomato soup",
                        PriceStudent = 2.00,
                        PriceExtern = 3.00,
                        Type = FoodType.Soep,
                        IsVegan = true,
                        IsVeggie = true
                    }
                }
            },
            new MenuDto.Index
            {
                Id = menu2Id,
                RestoId = Resto1Id,
                Date = firstDay.AddDays(1), // Next day
                MenuItems = new List<MenuItemDto.Index>
                {
                    new MenuItemDto.Index
                    {
                        Id = Guid.NewGuid(),
                        MenuId = menu2Id,
                        Name = "Chicken Curry",
                        Description = "Spicy chicken with rice",
                        PriceStudent = 5.00,
                        PriceExtern = 7.00,
                        Type = FoodType.WarmeMaaltijd,
                        IsVegan = false,
                        IsVeggie = false
                    },
                    new MenuItemDto.Index
                    {
                        Id = Guid.NewGuid(),
                        MenuId = menu2Id,
                        Name = "Vegan Buddha Bowl",
                        Description = "Healthy bowl with quinoa and veggies",
                        PriceStudent = 4.80,
                        PriceExtern = 6.80,
                        Type = FoodType.KoudeMaaltijd,
                        IsVegan = true,
                        IsVeggie = true
                    }
                }
            },
            new MenuDto.Index
            {
                Id = menu3Id,
                RestoId = Resto2Id,
                Date = firstDay,
                MenuItems = new List<MenuItemDto.Index>
                {
                    new MenuItemDto.Index
                    {
                        Id = Guid.NewGuid(),
                        MenuId = menu3Id,
                        Name = "Fish and Chips",
                        Description = "Fried fish with fries",
                        PriceStudent = 5.50,
                        PriceExtern = 7.50,
                        Type = FoodType.WarmeMaaltijd,
                        IsVegan = false,
                        IsVeggie = false
                    }
                }
            }
        };

        var response = new MenuResponse.Index
        {
            Menus = menus
        };

        return Task.FromResult(Result.Success(response));
    }
}
