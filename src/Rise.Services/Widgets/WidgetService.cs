using Microsoft.EntityFrameworkCore;
using Rise.Domain.HomeWidgets;
using Rise.Persistence;
using Rise.Services.Identity;
using Rise.Shared.Widgets;

namespace Rise.Services.Widgets;

public class WidgetService(ApplicationDbContext dbContext, ISessionContextProvider sessionContextProvider) : IWidgetService
{

    private string GetCurrentUserId()
    {
        var userId = sessionContextProvider.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return (string.IsNullOrEmpty(userId) ? null : userId) ?? string.Empty;
    }


    public async Task<Result<WidgetResponse.Index>> GetIndexByUserIdAsync(CancellationToken ctx)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Result.Success(new WidgetResponse.Index
            {
                UserWidgets = new List<UserWidgetDto.Index>()
            });
        }
        var query = await dbContext.UserWidgets
            .AsNoTracking()
            .Where(uw => uw.UserId == userId && !uw.IsDeleted)
            .Include(uw => uw.Widget)
            .ToListAsync(ctx);
        var userWidgets = query
            .Select(uw => new UserWidgetDto.Index
            {
                Id = uw.Id,
                //UserId = uw.UserId,
                X = uw.X,
                Y = uw.Y,
                Width = uw.Width,
                Height = uw.Height,
                MinWidth = uw.MinWidth,
                Widget = new WidgetDto.Index
                {
                    Id = uw.Widget!.Id,
                    Key = uw.Widget!.TypeName,
                }

            }).ToList();
        return Result.Success(new WidgetResponse.Index
        {
            UserWidgets = userWidgets
        });
    }

    public async Task<Result> UpdateUserWidgetsAsync(WidgetRequest.Update request,
        CancellationToken ctx)
    {

        var userId = GetCurrentUserId();

        var existingWidgets = await dbContext.UserWidgets.AsQueryable()
            .Where(uw => uw.UserId.Equals(userId) && !uw.IsDeleted)
            .ToListAsync(ctx);

        var incoming = request.UserWidgets ?? new List<UserWidgetDto.Update>();

        var incomingDict = incoming.ToDictionary(w => w.Id);

        var incomingIds = incomingDict.Keys.ToList();
        if (incomingIds.Any())
        {
            var owners = await dbContext.UserWidgets
                .Where(uw => incomingIds.Contains(uw.Id) && uw.IsDeleted == false)
                .Select(uw => new { uw.Id, uw.UserId })
                .ToListAsync(ctx);

            var notOwned = owners.Where(o => !o.UserId.Equals(userId)).Select(o => o.Id).ToList();
            if (notOwned.Any())
            {
                return Result.Forbidden("Attempt to modify widgets not owned by the current user.");
            }
        }

        foreach (var dbWidget in existingWidgets)
        {
            if (incomingDict.TryGetValue(dbWidget.Id, out var incomingWidget))
            {
                dbWidget.X = incomingWidget.X;
                dbWidget.Y = incomingWidget.Y;
                dbWidget.Width = incomingWidget.Width;
                dbWidget.Height = incomingWidget.Height;
                dbWidget.MinWidth = incomingWidget.MinWidth;

                // Remove from incomingDict so only new ones remain
                incomingDict.Remove(dbWidget.Id);
            }
            else
            {
                dbContext.UserWidgets.Remove(dbWidget);
            }
        }

        var widgetNames = incomingDict.Values.Select(w => w.WidgetName).Distinct().ToList();
        var widgetMap = await dbContext.Widgets
            .Where(w => widgetNames.Contains(w.TypeName.ToLower()))
            .ToDictionaryAsync(w => w.TypeName, w => w.Id, ctx);

        var missingWidgetTypes = widgetNames.Except(widgetMap.Keys).ToList();
        if (missingWidgetTypes.Any())
        {
            return Result.NotFound($"Widget type(s) not found: {string.Join(", ", missingWidgetTypes)}");
        }

        foreach (var newWidget in incomingDict.Values)
        {
            var widgetId = widgetMap[newWidget.WidgetName];
            dbContext.UserWidgets.Add(new UserWidget
            {
                WidgetId = widgetId,
                UserId = userId,
                X = newWidget.X,
                Y = newWidget.Y,
                Width = newWidget.Width,
                Height = newWidget.Height,
                MinWidth = newWidget.MinWidth
            });
        }


        await dbContext.SaveChangesAsync(ctx);

        return Result.Success();
    }
}
