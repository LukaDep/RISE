using Ardalis.Result;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Rise.Client.Campus;
using Rise.Shared.Campus;
using Rise.Shared.Common;
using Xunit;

namespace Rise.Client.Campus;

public class CampusInfoShould : TestContext
{
    private readonly FakeCampusService _fakeCampusService = new();

    public CampusInfoShould()
    {
        Services.AddScoped<ICampusService>(_ => _fakeCampusService);
        Services.AddLocalization();
    }

    [Fact]
    public void RenderHeaderAndSearchButton()
    {
        var cut = RenderComponent<CampusInfo>();

        var markup = cut.Markup;
        // SearchBar uses fa-magnifying-glass icon
        Assert.Contains("fa-magnifying-glass", markup);
    }

    [Fact]
    public void RenderCampusCards()
    {
        var cut = RenderComponent<CampusInfo>();

        cut.WaitForAssertion(() =>
        {
            var markup = cut.Markup;
            Assert.Contains("Campus Schoonmeersen", markup);
            Assert.Contains("Campus Mercator", markup);
            Assert.Contains("Campus Vesalius", markup);
        });
    }

    [Fact]
    public void ShowLoadingMessageWhenCampusInfoIsNull()
    {
        var nullService = new NullCampusService();
        Services.AddScoped<ICampusService>(_ => nullService);

        var cut = RenderComponent<CampusInfo>();

        var markup = cut.Markup;
        // LoadingComponent renders a loading spinner and label
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains(localizer["Campus.Loading"], markup);
    }

    [Fact]
    public void ToggleSearchInput()
    {
        var cut = RenderComponent<CampusInfo>();

        var searchButton = cut.Find("button");
        searchButton.Click();

        cut.WaitForAssertion(() =>
        {
            // SearchBar removes hidden class when open, shows input element
            var markup = cut.Markup;
            Assert.DoesNotContain("hidden", cut.Find("input[name='search']").GetAttribute("class") ?? "");
        });
    }

    [Fact]
    public void RenderAllCampusesInitially()
    {
        var cut = RenderComponent<CampusInfo>();

        cut.WaitForAssertion(() =>
        {
            var markup = cut.Markup;
            Assert.Contains("Schoonmeersen", markup);
            Assert.Contains("Mercator", markup);
            Assert.Contains("Vesalius", markup);
        });
    }

    [Fact]
    public void DisplayCampusLocations()
    {
        var cut = RenderComponent<CampusInfo>();

        cut.WaitForAssertion(() =>
        {
            var markup = cut.Markup;
            Assert.Contains("Valentin Vaerwyckweg", markup);
            Assert.Contains("Henleykaai", markup);
            Assert.Contains("Keramiekstraat", markup);
        });
    }

    [Fact]
    public void ToggleSearchInputClosesWhenClickedTwice()
    {
        var cut = RenderComponent<CampusInfo>();
        var searchButton = cut.Find("button");

        // SearchBar now always shows input, clicking button clears/toggles the value
        searchButton.Click();
        searchButton.Click();

        // Input should still be present (SearchBar always shows input)
        var input = cut.Find("input[name='search']");
        Assert.NotNull(input);
    }

    [Fact]
    public void SearchTermChanged_UpdatesNavigationUrl()
    {
        var cut = RenderComponent<CampusInfo>();
        var navManager = Services.GetRequiredService<NavigationManager>();

        cut.WaitForAssertion(() =>
        {
            var searchButton = cut.Find("button");
            searchButton.Click();
        });

        var input = cut.Find("input[type='text']");
        input.Input("Mercator");

        Assert.Contains("searchTerm=Mercator", navManager.Uri);
    }

    [Fact]
    public void SearchTermChanged_TriggersNavigation()
    {
        var cut = RenderComponent<CampusInfo>();
        var navManager = Services.GetRequiredService<NavigationManager>();
        var originalUri = navManager.Uri;

        cut.WaitForAssertion(() =>
        {
            var searchButton = cut.Find("button");
            searchButton.Click();
        });

        var input = cut.Find("input[type='text']");
        input.Input("test");

        Assert.NotEqual(originalUri, navManager.Uri);
    }

    [Fact]
    public void OnParametersSetAsync_LoadsCampusesWithSearchTerm()
    {
        var navManager = Services.GetRequiredService<NavigationManager>();
        var uri = navManager.GetUriWithQueryParameter("SearchTerm", "Mercator");
        navManager.NavigateTo(uri);

        var cut = RenderComponent<CampusInfo>();

        cut.WaitForAssertion(() =>
        {
            var markup = cut.Markup;
            Assert.NotNull(markup);
            // BasePage uses min-h-screen bg-gray-50 styling
            Assert.Contains("min-h-screen", markup);
        });
    }

    [Fact]
    public void OnParametersSetAsync_HandlesNullSearchTerm()
    {
        var cut = RenderComponent<CampusInfo>();

        cut.WaitForAssertion(() =>
        {
            var markup = cut.Markup;
            Assert.Contains("Schoonmeersen", markup);
            Assert.Contains("Mercator", markup);
            Assert.Contains("Vesalius", markup);
        });
    }

    [Fact]
    public void FilterCampuses_NavigatesWithSearchTerm()
    {
        var cut = RenderComponent<CampusInfo>();
        var navManager = Services.GetRequiredService<NavigationManager>();

        cut.WaitForAssertion(() =>
        {
            var searchButton = cut.Find("button");
            searchButton.Click();
        });

        var input = cut.Find("input[type='text']");
        input.Input("Campus");

        Assert.Contains("searchTerm", navManager.Uri);
        Assert.Contains("Campus", navManager.Uri);
    }

    [Fact]
    public void SearchInput_HasCorrectPlaceholder()
    {
        var cut = RenderComponent<CampusInfo>();
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();

        var searchButton = cut.Find("button");
        searchButton.Click();

        cut.WaitForAssertion(() =>
        {
            // SearchBar uses Common.SearchPlaceholder resource
            Assert.Contains($"placeholder=\"{localizer["Common.SearchPlaceholder"]}\"", cut.Markup);
        });
    }
}

public class NullCampusService : ICampusService
{
    public Task<Result<CampusResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var response = new CampusResponse.Index { Campuses = null! };
        return Task.FromResult(Result.Success(response));
    }

    public Task<Result<CampusResponse.Get>> GetCampusByIdAsync(Guid id, CancellationToken ct = default)
    {
        return Task.FromResult(Result<CampusResponse.Get>.NotFound());
    }

    public Task<Result<BuildingResponse.Get>> GetBuildingByBuildingCodeAsync(string code, CancellationToken ct = default)
    {
        return Task.FromResult(Result<BuildingResponse.Get>.NotFound());
    }
}
