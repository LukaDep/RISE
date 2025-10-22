using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.CampusInfo;

namespace Rise.Client.CampusInfo;

public partial class CampusInfo
{
    private IEnumerable<CampusInfoDto.Index>? campusInfo;

    private ElementReference filterInput;
    private bool isFilterOpen = false;

    [Parameter, SupplyParameterFromQuery]
    public string? SearchTerm { get; set; } = string.Empty;

    [Inject] public required ICampusInfoService CampusInfoService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await LoadCampusesAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        await LoadCampusesAsync();
    }

    private async Task LoadCampusesAsync()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 50,
            OrderBy = "Id",
            SearchTerm = SearchTerm ?? string.Empty
        };

        var result = await CampusInfoService.GetIndexAsync(request);
        campusInfo = result.Value.CampusInfo;
    }

    private async Task ToggleFilter()
    {
        isFilterOpen = !isFilterOpen;
        if (isFilterOpen)
        {
            await filterInput.FocusAsync();
        }
    }

    private async Task SearchTermChanged(ChangeEventArgs args)
    {
        var newValue = args.Value?.ToString() ?? string.Empty;

        if (newValue.Equals(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return;

        SearchTerm = newValue;

        var uri = NavigationManager.GetUriWithQueryParameters(
            new Dictionary<string, object?>
            {
                { nameof(SearchTerm), SearchTerm }
            });

        NavigationManager.NavigateTo(uri, forceLoad: false);

        await LoadCampusesAsync();
    }
}
