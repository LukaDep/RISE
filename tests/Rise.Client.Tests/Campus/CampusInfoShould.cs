using Ardalis.Result;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
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
        Assert.Contains("flex justify-between items-center", markup);
        Assert.Contains("fa-search", markup);
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
        Assert.Contains("<p>", markup);
    }

    [Fact]
    public void ToggleSearchInput()
    {
        var cut = RenderComponent<CampusInfo>();

        var searchButton = cut.Find("button");
        searchButton.Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("width:94%", cut.Markup);
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

        searchButton.Click();
        searchButton.Click();

        Assert.Contains("width:0px", cut.Markup);
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
            Assert.Contains("flex justify-between", markup);
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

        var searchButton = cut.Find("button");
        searchButton.Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("placeholder=", cut.Markup);
            Assert.Contains("Zoekterm", cut.Markup);
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
