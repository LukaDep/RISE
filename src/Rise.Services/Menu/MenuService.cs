using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.Common;
using Rise.Shared.Menu;
using Serilog;
namespace Rise.Services.Menu;

/// <summary>
/// Service for managing menus and menu items.
/// </summary>
public class MenuService(ApplicationDbContext dbContext) : IMenuService
{
    /// <summary>
    /// Retrieves a paginated list of menus including their menu items.
    /// </summary>
    /// <param name="request">QueryRequest.SkipTake with Skip and Take for pagination</param>
    /// <param name="ct">CancellationToken to cancel the operation</param>
    /// <returns>Result with MenuResponse.Index containing the list of menus with their items</returns>
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
