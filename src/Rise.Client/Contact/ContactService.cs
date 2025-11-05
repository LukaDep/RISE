using Rise.Shared.Contact;
using Rise.Shared.Common;
using System.Net.Http.Json;

public class ContactService(HttpClient httpClient) : IContactService
{
    public async Task<Result<ContactResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        //var result = await httpClient.GetFromJsonAsync<Result<ContactResponse.Index>>($"/api/contact?searchterm={request.SearchTerm}&skip={request.Skip}&take={request.Take}", cancellationToken: ctx);
        var result = await httpClient.GetFromJsonAsync<Result<ContactResponse.Index>>($"/api/contact?take=1000", cancellationToken: ctx);
        Console.Write(result);
        return result!;
    }

}