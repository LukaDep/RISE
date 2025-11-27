using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.Resto;

namespace Rise.Client.Resto;

public class IndexShould : TestContext
{
    public IndexShould()
    {
        Services.AddLocalization();
        Services.AddScoped<IRestoService, FakeRestoService>();
    }

    [Fact]
    public void RendersHeaderAndSearchElements()
    {
        var cut = RenderComponent<Index>();
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains("<h3", cut.Markup);
        Assert.Contains(localizer["Resto.Title"], cut.Markup);

        Assert.Contains($"placeholder=\"{localizer["Resto.FilterPlaceholder"]}\"", cut.Markup);
        Assert.Contains("fa-magnifying-glass", cut.Markup);
    }

    [Fact]
    public void ShowsSpinnerWhenRestosIsNull()
    {
        Services.AddScoped<IRestoService, NullRestoService>();
        var cut = RenderComponent<Index>();
        Assert.Contains("animate-spin", cut.Markup);
    }

    [Fact]
    public void ShowsNoResultsWhenEmpty()
    {
        var nav = Services.GetRequiredService<NavigationManager>();
        var uri = nav.GetUriWithQueryParameter("SearchTerm", "NoSuchPlace");
        nav.NavigateTo(uri);
        var cut = RenderComponent<Index>();
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains(localizer["Resto.NoResults"], cut.Markup);
        Assert.Contains("fa-triangle-exclamation", cut.Markup);
    }

    [Fact]
    public async Task RendersNavigationLinksToRestoMenus()
    {
        var cut = RenderComponent<Index>();

        var restoService = Services.GetRequiredService<IRestoService>();
        var result = await restoService.GetIndexAsync(new QueryRequest.SkipTake());
        var restos = result.Value.Restos!;

        cut.WaitForAssertion(() =>
        {
            foreach (var resto in restos)
            {
                var expectedHref = $"/resto/{resto.Id}/menu";
                Assert.Contains(expectedHref, cut.Markup);
            }
            var menuLinks = cut.FindAll($"a[href^='/resto/'][href$='/menu']");
            Assert.NotEmpty(menuLinks);
            Assert.Equal(restos.Count(), menuLinks.Count);
        });
    }
}
