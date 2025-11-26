using FuzzySharp;
using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.Resto;

namespace Rise.Client.Resto;

public partial class Index : ComponentBase
{
    ElementReference filterInput;
    private bool isFilterOpen = false;

    private async Task ToggleFilter()
    {
        isFilterOpen = !isFilterOpen;
        if (isFilterOpen)
        {
            await filterInput.FocusAsync();
        }
    }

    private IEnumerable<RestoDto.Index>? restos;

    [Inject] public required IRestoService RestoService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;

    [Parameter]
    [SupplyParameterFromQuery]
    public string? SearchTerm { get; set; } = default!;

    private string? searchTerm;
    private const int FuzzyScoreThreshold = 60;

    protected override async Task OnInitializedAsync()
    {
        await LoadRestosAsync();
    }

    private async Task LoadRestosAsync()
    {
        QueryRequest.SkipTake request = new()
        {
            Skip = 0,
            Take = 20,
            SearchTerm = SearchTerm ?? string.Empty,
        };

        var result = await RestoService.GetIndexAsync(request);

        if (result.IsSuccess)
        {
            restos = result.Value.Restos;
        }
        else
        {
            restos = new List<RestoDto.Index>();
        }

        searchTerm = SearchTerm;
    }
    private async Task SearchTermChanged(string value)
    {
        searchTerm = value;

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            await LoadRestosAsync();
        }
        else
        {
            QueryRequest.SkipTake request = new()
            {
                Skip = 0,
                Take = 100,
            };

            var result = await RestoService.GetIndexAsync(request);

            if (result.IsSuccess)
            {
                var allRestos = result.Value.Restos;

                restos = allRestos
                    .Select(r => new
                    {
                        Resto = r,
                        Score = Fuzz.WeightedRatio(searchTerm.ToLower(), r.Name.ToLower())
                    })
                    .Where(x => x.Score >= FuzzyScoreThreshold)
                    .OrderByDescending(x => x.Score)
                    .Select(x => x.Resto)
                    .ToList();
            }
        }

        StateHasChanged();
    }
}
