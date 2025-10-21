using System.Collections.Generic;
using System.Linq;
using FuzzySharp;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.News;

namespace Rise.Client.News;

public partial class Index
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

    private IEnumerable<NewsDto.Index>? news;


    [Inject] public required INewsService NewsService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;

    //dit id voor server-side filtering, als da er nog zou komen
    // [SupplyParameterFromQuery]
    // public string? SearchTerm { get; set; }

    [Parameter, EditorRequired]
    [SupplyParameterFromQuery]
    public string? SearchTerm { get; set; } = default!;

    private string? searchTerm;
    private const int FuzzyScoreThreshold = 60; // Minimum score for a match (0-100)

    protected override async Task OnParametersSetAsync()
    {
        QueryRequest.SkipTake request = new()
        {
            Skip = 0,
            Take = 20,
            SearchTerm = SearchTerm?.Length > 0 ? SearchTerm : "",
        };

        var result = await NewsService.GetIndexAsync(request);
        news = result.Value.News;

    }

    protected override void OnParametersSet()
    {
        searchTerm = SearchTerm;
    }

    private void SearchTermChanged(ChangeEventArgs args)
    {
        // When the inputfield changes...
        searchTerm = args.Value?.ToString();
        FilterProducts();
    }

    private void FilterProducts()
    { // Navigate to the current page with the new SearchTerm parameter.
        Dictionary<string, object?> parameters = new();
        parameters.Add(nameof(searchTerm), searchTerm);
        var uri = NavigationManager.GetUriWithQueryParameters(parameters);
        NavigationManager.NavigateTo(uri);
    }

    // private void SearchTermChanged(ChangeEventArgs e)
    // {
    //     searchTerm = e.Value?.ToString();
    //
    //     if (string.IsNullOrWhiteSpace(searchTerm))
    //     {
    //         filteredNews = news;
    //         return;
    //     }
    //
    //     filteredNews = news?.Where(n =>
    //         // Exact match still gets priority
    //         n.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
    //         // Fuzzy search
    //         Fuzz.PartialRatio(n.Title, searchTerm) >= FuzzyScoreThreshold ||
    //         Fuzz.PartialRatio(n.Content, searchTerm) >= FuzzyScoreThreshold
    //     ).OrderByDescending(n =>
    //         Math.Max(
    //             Fuzz.PartialRatio(n.Title, searchTerm),
    //             Fuzz.PartialRatio(n.Content, searchTerm)
    //         )
    //     );
    // }
}