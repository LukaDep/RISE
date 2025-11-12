using Rise.Shared.Common;
using Rise.Shared.Contact;
using System.Net.Http.Json;
using System.Text.Json;

public class ContactService(HttpClient httpClient) : IContactService
{
    public async Task<Result<ContactResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<ContactResponse.Index>>($"/api/contacts?searchterm={request.SearchTerm}&skip={request.Skip}&take={request.Take}&filters={JsonSerializer.Serialize(request.Filters)}", cancellationToken: ctx);
        return result!;
    }

}