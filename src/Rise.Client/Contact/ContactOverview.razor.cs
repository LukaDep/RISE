using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.Contact;
using System;
using System.Web;

namespace Rise.Client.Contact;
public partial class ContactOverview : ComponentBase, IDisposable
{
    [Inject] public required IContactService ContactService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;

    private IEnumerable<ContactDto.Index>? contacts;
    private IEnumerable<ContactDto.Index> filteredContacts => FilterContacts();

    public string? SearchTerm { get; set; }
    public string? Filter { get; set; }
    private List<string>? types;
    private bool isSearchOpen = false;
    private Guid? expandedContactId = null;

    private int skip = 0;
    private int take = 100;

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
            SearchTerm = "",
            Filters = new Dictionary<string, object?>()
            {
                { "Type", "" }
            }
        };

        return await ContactService.GetIndexAsync(request);
    }

    private IEnumerable<ContactDto.Index> FilterContacts()
    {
        if (contacts == null) return Enumerable.Empty<ContactDto.Index>();

        var result = contacts.AsEnumerable();

        // Filter by type
        if (!string.IsNullOrEmpty(Filter))
        {
            result = result.Where(c => c.Type == Filter);
        }

        // Filter by search term
        if (!string.IsNullOrEmpty(SearchTerm))
        {
            var searchLower = SearchTerm.ToLower();
            result = result.Where(c =>
                c.Name.ToLower().Contains(searchLower) ||
                (c.Email?.ToLower().Contains(searchLower) ?? false) ||
                (c.ContactPerson?.ToLower().Contains(searchLower) ?? false) ||
                c.Type.ToLower().Contains(searchLower));
        }

        return result;
    }

    private void SearchTermChanged(string value)
    {
        SearchTerm = value;
        StateHasChanged();
    }

    private void SetFilter(string? filter)
    {
        Filter = filter;
        expandedContactId = null;
        StateHasChanged();
    }

    private void ToggleContact(Guid contactId)
    {
        expandedContactId = expandedContactId == contactId ? null : contactId;
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
        StateHasChanged();
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
    }
}
