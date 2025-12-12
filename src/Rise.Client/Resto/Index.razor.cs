using FuzzySharp;
using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.Resto;

namespace Rise.Client.Resto;

/// <summary>
/// Code-behind for the Resto Index page component.
/// Displays a searchable list of restaurants with fuzzy search capability.
/// </summary>
public partial class Index : ComponentBase
{
    ElementReference filterInput;
    private bool isFilterOpen = false;

    /// <summary>
    /// Toggles the search filter visibility and focuses the input.
    /// </summary>
    private async Task ToggleFilter()
    {
        isFilterOpen = !isFilterOpen;
        if (isFilterOpen)
        {
            await filterInput.FocusAsync();
        }
    }

    private IEnumerable<RestoDto.Index>? restos;

    /// <summary>Service for restaurant data operations.</summary>
    [Inject] public required IRestoService RestoService { get; set; }
    
    /// <summary>Navigation manager for URL handling.</summary>
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;

    /// <summary>Search term from query string.</summary>
    [Parameter]
    [SupplyParameterFromQuery]
    public string? SearchTerm { get; set; } = default!;

    private string? searchTerm;
    private const int FuzzyScoreThreshold = 60;

    /// <summary>
    /// Initializes the component and loads restaurant data.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        await LoadRestosAsync();
    }

    /// <summary>
    /// Loads restaurant data from the service.
    /// </summary>
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
    /// <summary>
    /// Handles search term changes with fuzzy search filtering.
    /// </summary>
    /// <param name="value">The new search term value.</param>
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
