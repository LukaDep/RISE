using System;
using Ardalis.Result;
using Rise.Shared.Common;
using Rise.Shared.News;

namespace Rise.Client.News;

public class NullNewsService : INewsService
{
    public Task<Result<NewsResponse.Index>> GetIndexAsync(QueryRequest.DateRange request, CancellationToken ctx = default)
    {
        var wrapper = new NewsResponse.Index
        {
            News = null!
        };

        return Task.FromResult(Result.Success(wrapper));
    }

    public Task<Result<NewsResponse.Get>> GetByIdAsync(Guid id, CancellationToken ctx = default)
    {
        return Task.FromResult(Result<NewsResponse.Get>.NotFound($"News item with id {id} not found."));
    }
}

public class FakeNewsService : INewsService
{
    private readonly List<NewsDto.Index> _items = new()
{
    new NewsDto.Index { Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Title = "Campus reopens", PublishDate = DateTime.Now.AddDays(-3), Type = "test1", Description = "tester1", Content = "We are happy to announce the campus reopens.", Author = "Admin" },
    new NewsDto.Index { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Title = "New library hours", PublishDate = DateTime.Now.AddDays(-2), Type = "test2", Description = "tester2", Content = "Library hours have changed for the exam period.", Author = "Library" },
    new NewsDto.Index { Id = new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), Title = "Cafeteria menu updated", PublishDate = DateTime.Now.AddDays(-1), Type = "test3", Description = "tester3", Content = "Try the new vegetarian options at the cafeteria.", Author = "Catering" },
    new NewsDto.Index { Id = new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), Title = "Guest lecture series", PublishDate = DateTime.Now, Type = "test4", Description = "tester4", Content = "A new guest lecture series will start next week.", Author = "Events" },
};

    public Task<Result<NewsResponse.Index>> GetIndexAsync(QueryRequest.DateRange request, CancellationToken ctx = default)
    {
        var query = _items.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request?.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            query = query.Where(n => n.Title.Contains(term, StringComparison.OrdinalIgnoreCase)
                         || n.Content.Contains(term, StringComparison.OrdinalIgnoreCase));
        }
        query = query.OrderByDescending(n => n.PublishDate);
        var skip = Math.Max(0, request?.Skip ?? 0);
        var take = Math.Max(0, request?.Take ?? 20);

        var page = query.Skip(skip).Take(take).ToList();

        var wrapper = new NewsResponse.Index
        {
            News = page
        };

        return Task.FromResult(Result.Success(wrapper));
    }

    public Task<Result<NewsResponse.Get>> GetByIdAsync(Guid id, CancellationToken ctx = default)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item is null)
            return Task.FromResult(Result<NewsResponse.Get>.NotFound($"News item with id {id} not found."));

        var wrapper = new NewsResponse.Get
        {
            NewsArticle = item
        };

        return Task.FromResult(Result.Success(wrapper));
    }
}