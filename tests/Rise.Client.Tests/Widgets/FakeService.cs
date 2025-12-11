using Ardalis.Result;
using Rise.Shared.Widgets;

namespace Rise.Client.Tests.Widgets;

public class NullWidgetService : IWidgetService
{
    public Task<Result<WidgetResponse.Index>> GetIndexByUserIdAsync(CancellationToken ctx = default)
    {
        var wrapper = new WidgetResponse.Index
        {
            UserWidgets = null!
        };
        return Task.FromResult(Result.Success(wrapper));
    }

    public Task<Result> UpdateUserWidgetsAsync(WidgetRequest.Update request, CancellationToken ctx = default)
    {
        throw new NotImplementedException();
    }
}

public class FakeWidgetService : IWidgetService
{
    private static readonly List<WidgetDto.Index> Widgets =
    [
        new WidgetDto.Index() { Id = Guid.NewGuid(), Key = "Widget A" },
        new WidgetDto.Index() { Id = Guid.NewGuid(), Key = "Widget B" },
        new WidgetDto.Index() { Id = Guid.NewGuid(), Key = "Widget C" }
    ];
    private readonly List<UserWidgetDto.Index> _items = new()
    {
        new UserWidgetDto.Index() { Id = Guid.NewGuid(), Widget = Widgets[0], X = 0,Y = 3, Width = 6, Height = 4, MinWidth = 4 },
        new UserWidgetDto.Index() { Id = Guid.NewGuid(), Widget = Widgets[1], X = 6,Y = 0, Width = 6, Height = 8, MinWidth = 4 },
        new UserWidgetDto.Index() { Id = Guid.NewGuid(), Widget = Widgets[2], X = 0,Y = 0, Width = 6, Height = 3, MinWidth = 4 }
    };

    public Task<Result<WidgetResponse.Index>> GetIndexByUserIdAsync(CancellationToken ctx = default)
    {
        var query = _items.AsEnumerable();
        var wrapper = new WidgetResponse.Index
        {
            UserWidgets = query.ToList()
        };
        return Task.FromResult(Result.Success(wrapper));
    }

    public Task<Result> UpdateUserWidgetsAsync(WidgetRequest.Update request, CancellationToken ctx = default)
    {
        throw new NotImplementedException();
    }
}