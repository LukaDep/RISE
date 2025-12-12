using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.Contact;
using System;
using System.Web;

namespace Rise.Client.Contact;

/// <summary>
/// Code-behind for the ContactOverview page component.
/// Displays a searchable and filterable list of contacts.
/// </summary>
public partial class ContactOverview : ComponentBase, IDisposable
{
    /// <summary>Service for contact data operations.</summary>
    [Inject] public required IContactService ContactService { get; set; }
    
    /// <summary>Navigation manager for URL handling.</summary>
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;

    private IEnumerable<ContactDto.Index>? contacts;
    private IEnumerable<ContactDto.Index> filteredContacts => FilterContacts();

    /// <summary>Current search term for filtering contacts.</summary>
    public string? SearchTerm { get; set; }
    
    /// <summary>Current type filter.</summary>
    public string? Filter { get; set; }
    
    private List<string>? types;
    private bool isSearchOpen = false;
    private Guid? expandedContactId = null;

    private int skip = 0;
    private int take = 100;

    /// <summary>
    /// Loads contact data when parameters change.
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        ContactResponse.Index result = await GetData();
        contacts = result.Contact;
        types = result.Contact.Select(x => x.Type).Distinct().ToList();
        skip = 0;
    }

    /// <summary>
    /// Fetches contact data from the service.
    /// </summary>
    /// <returns>The contact response with contact list.</returns>
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

    /// <summary>
    /// Filters contacts based on search term and type filter.
    /// </summary>
    /// <returns>Filtered list of contacts.</returns>
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

    /// <summary>
    /// Handles search term changes and triggers re-render.
    /// </summary>
    /// <param name="value">The new search term.</param>
    private void SearchTermChanged(string value)
    {
        SearchTerm = value;
        StateHasChanged();
    }

    /// <summary>
    /// Sets the contact type filter.
    /// </summary>
    /// <param name="filter">The type filter to apply, or null for all types.</param>
    private void SetFilter(string? filter)
    {
        Filter = filter;
        expandedContactId = null;
        StateHasChanged();
    }

    /// <summary>
    /// Toggles the expanded state of a contact card.
    /// </summary>
    /// <param name="contactId">The ID of the contact to toggle.</param>
    private void ToggleContact(Guid contactId)
    {
        expandedContactId = expandedContactId == contactId ? null : contactId;
    }

    /// <summary>
    /// Initializes the component and subscribes to location change events.
    /// </summary>
    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += OnLocationChanged;
    }

    /// <summary>
    /// Handles location changes to update the filter from query parameters.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The location changed event arguments.</param>
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
