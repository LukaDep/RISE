using Rise.Shared.Common;
using Rise.Shared.Contact;
using System.Net.Http.Json;
using System.Text.Json;

/// <summary>
/// Client-side service for contact operations.
/// </summary>
/// <param name="httpClient">The HTTP client for API communication.</param>
public class ContactService(HttpClient httpClient) : IContactService
{
    /// <summary>
    /// Retrieves a paginated and filtered list of contacts.
    /// </summary>
    /// <param name="request">The request containing pagination, search, and filter options.</param>
    /// <param name="ctx">Cancellation token.</param>
    /// <returns>A result containing the list of contacts.</returns>
    public async Task<Result<ContactResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<ContactResponse.Index>>($"/api/contacts?searchterm={request.SearchTerm}&skip={request.Skip}&take={request.Take}&filters={JsonSerializer.Serialize(request.Filters)}", cancellationToken: ctx);
        return result!;
    }

}