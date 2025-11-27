using System;
using Ardalis.Result;
using Rise.Shared.Common;
using Rise.Shared.Resto;

namespace Rise.Client.Resto;

public class NullRestoService : IRestoService
{
    public Task<Result<RestoResponse.Index>> GetIndexAsync(QueryRequest.SkipTake req, CancellationToken ctx = default)
    {
        var wrapper = new RestoResponse.Index
        {
            Restos = null!
        };

        return Task.FromResult(Result.Success(wrapper));
    }
}

public class FakeRestoService : IRestoService
{
    private readonly List<RestoDto.Index> _items = new()
    {
        new RestoDto.Index
        {
            Id = Guid.CreateVersion7(),
            Name = "Campus Cafe",
            BuildingId = Guid.CreateVersion7(),
            Description = "Coffee and snacks",
            KitchenType = new List<string> { "Coffee", "Snacks" },
            IsCurrentlyOpen = true,
            OpeningHours = new Dictionary<DayOfWeek, string>
            {
                { DayOfWeek.Monday, "09:00-17:00" },
                { DayOfWeek.Tuesday, "09:00-17:00" },
            },
            ImageUrl = "https://example.com/campus-cafe.jpg",
            PhoneNumber = "+1 555-0100",
            Email = "campus@cafe.test"
        },
        new RestoDto.Index
        {
            Id = Guid.CreateVersion7(),
            Name = "Library Bistro",
            BuildingId = Guid.CreateVersion7(),
            Description = "Lunch and salads",
            KitchenType = new List<string> { "Lunch" },
            IsCurrentlyOpen = false,
            OpeningHours = new Dictionary<DayOfWeek, string>
            {
                { DayOfWeek.Wednesday, "10:00-16:00" }
            }
        },
        new RestoDto.Index
        {
            Id = Guid.CreateVersion7(),
            Name = "Tech Lounge",
            BuildingId = Guid.CreateVersion7(),
            Description = "Drinks and quick bites",
            KitchenType = new List<string> { "Drinks" },
            IsCurrentlyOpen = false
        }
    };

    public Task<Result<RestoResponse.Index>> GetIndexAsync(QueryRequest.SkipTake req, CancellationToken ctx = default)
    {
        var query = _items.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(req?.SearchTerm))
        {
            var term = req.SearchTerm.Trim();
            query = query.Where(r => r.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                                   || (r.Description?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        var skip = Math.Max(0, req?.Skip ?? 0);
        var take = Math.Max(0, req?.Take ?? 20);

        var page = query
            .OrderBy(r => r.Name)
            .Skip(skip)
            .Take(take)
            .ToList();

        var wrapper = new RestoResponse.Index
        {
            Restos = page
        };

        return Task.FromResult(Result.Success(wrapper));
    }
}
