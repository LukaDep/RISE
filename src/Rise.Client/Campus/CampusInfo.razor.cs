using Microsoft.AspNetCore.Components;
using Rise.Shared.Campus;
using Rise.Shared.Common;

namespace Rise.Client.Campus;

/// <summary>
/// Code-behind for the CampusInfo page component.
/// Displays a searchable list of campuses with their details.
/// </summary>
public partial class CampusInfo : ComponentBase
{
    private IEnumerable<CampusDto.Index>? campusInfo;

    private ElementReference filterInput;
    private bool isFilterOpen = false;
    
    /// <summary>
    /// Toggles the filter input visibility and focuses it when opened.
    /// </summary>
    private async Task ToggleFilter()
    {
        isFilterOpen = !isFilterOpen;
        if (isFilterOpen)
        {
            await filterInput.FocusAsync();
        }
    }

    /// <summary>Search term from the query string.</summary>
    [Parameter, SupplyParameterFromQuery]
    public string? SearchTerm { get; set; } = default!;

    private string? searchTerm;

    /// <summary>Service for campus data operations.</summary>
    [Inject] public required ICampusService CampusService { get; set; }
    
    /// <summary>Navigation manager for URL handling.</summary>
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;

    /// <summary>
    /// Loads campus data when parameters change.
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        QueryRequest.SkipTake request = new()
        {
            Skip = 0,
            Take = 20,
            SearchTerm = SearchTerm?.Length > 0 ? SearchTerm : "",
        };

        var result = await CampusService.GetIndexAsync(request);
        campusInfo = result.Value.Campuses;

    }

    /// <summary>
    /// Handles search term changes and triggers filtering.
    /// </summary>
    /// <param name="value">The new search term.</param>
    private void SearchTermChanged(string value)
    {
        searchTerm = value;
        FilterProducts();
    }

    /// <summary>
    /// Updates the URL with the current search term and navigates.
    /// </summary>
    private void FilterProducts()
    {
        Dictionary<string, object?> parameters = new();
        parameters.Add(nameof(searchTerm), searchTerm);
        var uri = NavigationManager.GetUriWithQueryParameters(parameters);
        NavigationManager.NavigateTo(uri);
    }
}
