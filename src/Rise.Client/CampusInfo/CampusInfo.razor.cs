using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.CampusInfo;

namespace Rise.Client.CampusInfo;

public partial class CampusInfo : ComponentBase
{
    private IEnumerable<CampusInfoDto.Index>? campusInfo;

    private ElementReference filterInput;
    private bool isFilterOpen = false;
    private async Task ToggleFilter()
    {
        isFilterOpen = !isFilterOpen;
        if (isFilterOpen)
        {
            await filterInput.FocusAsync();
        }
    }

    [Parameter, SupplyParameterFromQuery]
    public string? SearchTerm { get; set; } = default!;

    private string? searchTerm;

    [Inject] public required ICampusInfoService CampusInfoService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        QueryRequest.SkipTake request = new()
        {
            Skip = 0,
            Take = 20,
            SearchTerm = SearchTerm?.Length > 0 ? SearchTerm : "",
        };

        var result = await CampusInfoService.GetIndexAsync(request);
        campusInfo = result.Value.CampusInfo;

    }

    private void SearchTermChanged(ChangeEventArgs args)
    {
        searchTerm = args.Value?.ToString();
        FilterProducts();
    }

    private void FilterProducts()
    {
        Dictionary<string, object?> parameters = new();
        parameters.Add(nameof(searchTerm), searchTerm);
        var uri = NavigationManager.GetUriWithQueryParameters(parameters);
        NavigationManager.NavigateTo(uri);
    }
}
