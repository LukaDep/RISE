using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.Contact;
using System.Web;

namespace Rise.Client.Contact;
public partial class ContactOverview : ComponentBase
{
    [Inject] public required IContactService ContactService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;

    public ContactDto.Index SelectedContact = null;

    private IEnumerable<ContactDto.Index>? contacts;

    public string? SearchTerm { get; set; }
    public string? Filter { get; set; }
    private List<string>? types;

    private int skip = 0;
    private int take = 100;
    private int totalCount;
    private int currentCount;

    protected override async Task OnParametersSetAsync()
    {
        ContactResponse.Index result = await GetData();
        contacts = result.Contact;
        types = result.Contact.Select(x => x.Type).Distinct().ToList();
        skip = 0;

    }

    protected async Task<ContactResponse.Index> GetData()
    {
        QueryRequest.SkipTake request = new()
        {
            Skip = skip,
            Take = take,
            SearchTerm = SearchTerm ?? "",
            Filters = new Dictionary<string, object?>()
            {
                { "Type", Filter ?? "" }
            }
        };

        return await ContactService.GetIndexAsync(request);
    }

    protected async void refreshData()
    {
        ContactResponse.Index result = await GetData();
        contacts = result.Contact;
        StateHasChanged();
    }


    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += OnLocationChanged;
    }

    private void OnLocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var currentQueryParams = HttpUtility.ParseQueryString(uri.Query);
        Filter = currentQueryParams.Get("filter") ?? "";
        refreshData();
    }

    private void ShowDetails(ContactDto.Index contact)
    {
        SelectedContact = contact;
    }

    private void CloseDetails()
    {
        SelectedContact = null;
    }
}
