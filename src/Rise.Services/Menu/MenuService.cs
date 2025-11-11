using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.Common;
using Rise.Shared.Menu;
using Serilog;
namespace Rise.Services.Menu;

/// <summary>
/// Mock service voor Menu's — leest JSON bestand en geeft lijst met menu's terug.
/// </summary>
public class MenuService(ApplicationDbContext dbContext) : IMenuService
{

    public async Task<Result<MenuResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ct = default)
    {
        var query = await dbContext.Menus
            .AsNoTracking()
            .Include(c => c.MenuItems)
            .Skip(request.Skip)
            .Take(request.Take)
            .ToListAsync(ct);

        var menus = query
            .Select(menu => new MenuDto.Index
            {
                Id = menu.Id,
                RestoId = menu.RestoId,
                Date = menu.Date,
                MenuItems = menu.MenuItems.Select(item => new MenuItemDto.Index
                {
                    Id = item.Id,
                    MenuId = menu.Id,
                    Name = item.Name,
                    Description = item.Description,
                    PriceStudent = item.PriceStudent,
                    PriceExtern = item.PriceExtern,
                    Type = item.Type,
                    IsVegan = item.IsVegan,
                    IsVeggie = item.IsVeggie
                }).ToList()
            }).ToList();

        return Result.Success(new MenuResponse.Index
        {
            Menus = menus
        }
        );
    }
}
