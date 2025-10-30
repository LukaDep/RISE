using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.Contact;

namespace Rise.Client.Contact;
public partial class ContactOverview : ComponentBase
{
    [Inject] public required IContactService ContactService { get; set; }
    
    private IEnumerable<ContactDto.Index>? contacts;

    public string? SearchTerm { get; set; }

    private int skip = 0;
    private int take = 10;
    private int totalCount;
    private int currentCount;

    protected override async Task OnParametersSetAsync()
    {
        QueryRequest.SkipTake request = new()
        {
            Skip = 0,
            Take = 10,
            SearchTerm = SearchTerm,
        };

        var result = await ContactService.GetIndexAsync(request);
        contacts = result.Value.Contact;
        skip = 0;

    }
}
