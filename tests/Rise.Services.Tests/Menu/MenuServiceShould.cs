using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Services.Menu;
using Rise.Shared.Common;
using MenuEntity = Rise.Domain.Menu.Menu;
using MenuItemEntity = Rise.Domain.Menu.MenuItem;

namespace Rise.Services.Tests.Menu;

public class MenuServiceShould
{
    [Fact]
    public async Task GetIndexAsync_Should_Return_Data()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: nameof(GetIndexAsync_Should_Return_Data))
            .Options;
        using var db = new ApplicationDbContext(options);

        var restoId = Guid.NewGuid();

        var menu1 = new MenuEntity
        {
            RestoId = restoId,
            Date = DateTime.Today.ToUniversalTime(),
            MenuItems = new List<MenuItemEntity>
            {
                new MenuItemEntity
                {
                    Name = "Pasta Carbonara",
                    Description = "Creamy pasta dish",
                    PriceStudent = 4.50,
                    PriceExtern = 6.50,
                    Type = FoodType.WarmeMaaltijd,
                    IsVegan = false,
                    IsVeggie = false
                },
                new MenuItemEntity
                {
                    Name = "Vegetable Curry",
                    Description = "Spicy vegetable curry",
                    PriceStudent = 4.20,
                    PriceExtern = 6.20,
                    Type = FoodType.WarmeMaaltijd,
                    IsVegan = false,
                    IsVeggie = true
                }
            }
        };

        var menu2 = new MenuEntity
        {
            RestoId = restoId,
            Date = DateTime.Today.AddDays(1).ToUniversalTime(),
            MenuItems = new List<MenuItemEntity>
            {
                new MenuItemEntity
                {
                    Name = "Tomato Soup",
                    Description = "Fresh tomato soup",
                    PriceStudent = 2.00,
                    PriceExtern = 3.00,
                    Type = FoodType.Soep,
                    IsVegan = true,
                    IsVeggie = true
                }
            }
        };

        db.Menus.AddRange(menu1, menu2);
        await db.SaveChangesAsync();

        var service = new MenuService(db);
        var req = new QueryRequest.SkipTake { Skip = 0, Take = 10 };

        var result = await service.GetIndexAsync(req, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.ShouldNotBeNull();
        result.Value.Menus.ShouldNotBeNull();
        result.Value.Menus.Count().ShouldBe(2);
    }

    [Fact]
    public async Task GetIndexAsync_Should_Include_MenuItems()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: nameof(GetIndexAsync_Should_Include_MenuItems))
            .Options;
        using var db = new ApplicationDbContext(options);

        var restoId = Guid.NewGuid();

        var menu = new MenuEntity
        {
            RestoId = restoId,
            Date = DateTime.Today.ToUniversalTime(),
            MenuItems = new List<MenuItemEntity>
            {
                new MenuItemEntity
                {
                    Name = "Spaghetti",
                    Description = "Italian pasta",
                    PriceStudent = 4.00,
                    PriceExtern = 6.00,
                    Type = FoodType.Pasta,
                    IsVegan = false,
                    IsVeggie = true
                },
                new MenuItemEntity
                {
                    Name = "Salad",
                    Description = "Fresh salad",
                    PriceStudent = 3.50,
                    PriceExtern = 5.50,
                    Type = FoodType.KoudeMaaltijd,
                    IsVegan = true,
                    IsVeggie = true
                }
            }
        };

        db.Menus.Add(menu);
        await db.SaveChangesAsync();

        var service = new MenuService(db);
        var req = new QueryRequest.SkipTake { Skip = 0, Take = 10 };

        var result = await service.GetIndexAsync(req, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Ok);
        var returnedMenu = result.Value.Menus.First();
        returnedMenu.MenuItems.ShouldNotBeNull();
        returnedMenu.MenuItems.Count().ShouldBe(2);
        returnedMenu.MenuItems.Any(i => i.Name == "Spaghetti").ShouldBeTrue();
        returnedMenu.MenuItems.Any(i => i.Name == "Salad").ShouldBeTrue();
    }

    [Fact]
    public async Task GetIndexAsync_Should_Return_Empty_When_No_Data()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: nameof(GetIndexAsync_Should_Return_Empty_When_No_Data))
            .Options;
        using var db = new ApplicationDbContext(options);

        var service = new MenuService(db);
        var req = new QueryRequest.SkipTake { Skip = 0, Take = 10 };

        var result = await service.GetIndexAsync(req, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.Menus.ShouldNotBeNull();
        result.Value.Menus.Count().ShouldBe(0);
    }

    [Fact]
    public async Task GetIndexAsync_Should_Map_Properties_Correctly()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: nameof(GetIndexAsync_Should_Map_Properties_Correctly))
            .Options;
        using var db = new ApplicationDbContext(options);

        var restoId = Guid.NewGuid();

        var menu = new MenuEntity
        {
            RestoId = restoId,
            Date = new DateTime(2024, 5, 15, 0, 0, 0, DateTimeKind.Utc),
            MenuItems = new List<MenuItemEntity>
            {
                new MenuItemEntity
                {
                    Name = "Chicken Tikka",
                    Description = "Spicy chicken dish",
                    PriceStudent = 5.00,
                    PriceExtern = 7.50,
                    Type = FoodType.WarmeMaaltijd,
                    IsVegan = false,
                    IsVeggie = false
                }
            }
        };

        db.Menus.Add(menu);
        await db.SaveChangesAsync();

        var service = new MenuService(db);
        var req = new QueryRequest.SkipTake { Skip = 0, Take = 10 };

        var result = await service.GetIndexAsync(req, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Ok);
        var dto = result.Value.Menus.First();
        dto.Id.ShouldNotBe(Guid.Empty);
        dto.RestoId.ShouldBe(restoId);
        dto.Date.Year.ShouldBe(2024);
        dto.Date.Month.ShouldBe(5);
        dto.Date.Day.ShouldBe(15);

        var itemDto = dto.MenuItems.First();
        itemDto.Id.ShouldNotBe(Guid.Empty);
        itemDto.MenuId.ShouldBe(dto.Id);
        itemDto.Name.ShouldBe("Chicken Tikka");
        itemDto.Description.ShouldBe("Spicy chicken dish");
        itemDto.PriceStudent.ShouldBe(5.00);
        itemDto.PriceExtern.ShouldBe(7.50);
        itemDto.Type.ShouldBe(FoodType.WarmeMaaltijd);
        itemDto.IsVegan.ShouldBeFalse();
        itemDto.IsVeggie.ShouldBeFalse();
    }

    [Fact]
    public async Task GetIndexAsync_Should_Respect_Skip_And_Take()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: nameof(GetIndexAsync_Should_Respect_Skip_And_Take))
            .Options;
        using var db = new ApplicationDbContext(options);

        var restoId = Guid.NewGuid();

        // Add 5 menus
        for (int i = 0; i < 5; i++)
        {
            db.Menus.Add(new MenuEntity
            {
                RestoId = restoId,
                Date = DateTime.Today.AddDays(i).ToUniversalTime(),
                MenuItems = new List<MenuItemEntity>
                {
                    new MenuItemEntity
                    {
                        Name = $"Item {i}",
                        Description = "Description",
                        PriceStudent = 4.00,
                        PriceExtern = 6.00,
                        Type = FoodType.WarmeMaaltijd
                    }
                }
            });
        }
        await db.SaveChangesAsync();

        var service = new MenuService(db);
        var req = new QueryRequest.SkipTake { Skip = 2, Take = 2 };

        var result = await service.GetIndexAsync(req, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.Menus.Count().ShouldBe(2);
    }

    [Fact]
    public async Task GetIndexAsync_Should_Handle_Menu_Without_Items()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: nameof(GetIndexAsync_Should_Handle_Menu_Without_Items))
            .Options;
        using var db = new ApplicationDbContext(options);

        var restoId = Guid.NewGuid();

        var menu = new MenuEntity
        {
            RestoId = restoId,
            Date = DateTime.Today.ToUniversalTime(),
            MenuItems = new List<MenuItemEntity>()
        };

        db.Menus.Add(menu);
        await db.SaveChangesAsync();

        var service = new MenuService(db);
        var req = new QueryRequest.SkipTake { Skip = 0, Take = 10 };

        var result = await service.GetIndexAsync(req, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Ok);
        var dto = result.Value.Menus.First();
        dto.MenuItems.ShouldNotBeNull();
        dto.MenuItems.Count().ShouldBe(0);
    }
}
