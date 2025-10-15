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
    private IEnumerable<NewsDto.Index>? filteredNews;

    [Inject] public required INewsService NewsService { get; set; }

    //dit id voor server-side filtering, als da er nog zou komen
    // [SupplyParameterFromQuery]
    // public string? SearchTerm { get; set; }

    private string? searchTerm;
    private const int FuzzyScoreThreshold = 60; // Minimum score for a match (0-100)

    protected override async Task OnInitializedAsync()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 50,
            OrderBy = "Id",
            //SearchTerm = SearchTerm
        };

        var result = await NewsService.GetIndexAsync(request);
        news = result.Value.News;
        filteredNews = news;
    }

    private void SearchTermChanged(ChangeEventArgs e)
    {
        searchTerm = e.Value?.ToString();

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            filteredNews = news;
            return;
        }

        filteredNews = news?.Where(n =>
            // Exact match still gets priority
            n.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            // Fuzzy search
            Fuzz.PartialRatio(n.Title, searchTerm) >= FuzzyScoreThreshold ||
            Fuzz.PartialRatio(n.Content, searchTerm) >= FuzzyScoreThreshold
        ).OrderByDescending(n =>
            Math.Max(
                Fuzz.PartialRatio(n.Title, searchTerm),
                Fuzz.PartialRatio(n.Content, searchTerm)
            )
        );
    }
}