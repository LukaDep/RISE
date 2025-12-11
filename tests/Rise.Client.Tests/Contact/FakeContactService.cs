namespace Rise.Client.Tests.Contact;

using Rise.Shared.Common;
using Rise.Shared.Contact;
using Ardalis.Result;

public class FakeContactService : IContactService
{
    private readonly List<ContactDto.Index> _items = new()
    {
        new ContactDto.Index { Id = Guid.NewGuid(), Type = "Academic", Name = "John Doe", Email = "john@example.org", PhoneNumber = "+123456", ContactPerson = "", Campusses = new[] { "Campus A" } },
        new ContactDto.Index { Id = Guid.NewGuid(), Type = "Administrative", Name = "Jane Smith", Email = "jane@example.org", PhoneNumber = null, ContactPerson = "Dept Lead", Campusses = new[] { "Campus B" } },
        new ContactDto.Index { Id = Guid.NewGuid(), Type = "Academic", Name = "Albert Chem", Email = "albert@example.org", PhoneNumber = "+987654", ContactPerson = "Coordinator", Campusses = new[] { "Campus A", "Campus B" } }
    };

    public Task<Result<ContactResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var query = _items.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request?.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            query = query.Where(c => c.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                          || (c.Email ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase)
                          || (c.ContactPerson ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase)
                          || c.Type.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        if (request?.Filters != null && request.Filters.TryGetValue("Type", out var typeObj) && typeObj is string type && !string.IsNullOrEmpty(type))
        {
            query = query.Where(c => c.Type == type);
        }

        var page = query.Skip(request?.Skip ?? 0).Take(request?.Take ?? 20).ToList();

        var wrapper = new ContactResponse.Index
        {
            Contact = page
        };

        return Task.FromResult(Result.Success(wrapper));
    }
}

public class NullContactService : IContactService
{
    public Task<Result<ContactResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var wrapper = new ContactResponse.Index
        {
            Contact = Enumerable.Empty<ContactDto.Index>()
        };

        return Task.FromResult(Result.Success(wrapper));
    }
}
