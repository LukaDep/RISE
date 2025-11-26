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
        // Arrange & Act
        var cut = RenderComponent<Index>();

        // Assert header/title rendered from localizer
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains("<h3", cut.Markup);
        Assert.Contains(localizer["Resto.Title"], cut.Markup);

        Assert.Contains($"placeholder=\"{localizer["Resto.FilterPlaceholder"]}\"", cut.Markup);

        // Assert the search button/icon exists
        Assert.Contains("fa-magnifying-glass", cut.Markup);
    }

    [Fact]
    public void ShowsSpinnerWhenRestosIsNull()
    {
        // Arrange: override the registered IRestoService so it returns a response with a null Restos list.
        Services.AddScoped<IRestoService, NullRestoService>();

        // Act
        var cut = RenderComponent<Index>();

        // The component shows a loading spinner when restos == null
        Assert.Contains("animate-spin", cut.Markup);
    }

    [Fact]
    public void ShowsNoResultsWhenEmpty()
    {
        // Arrange: navigate with a term that won't match any item (server-side filtering path on initial load)
        var nav = Services.GetRequiredService<NavigationManager>();
        var uri = nav.GetUriWithQueryParameter("SearchTerm", "NoSuchPlace");
        nav.NavigateTo(uri);

        // Act
        var cut = RenderComponent<Index>();

        // Assert the no-results panel is shown
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

            // Verify at least one menu link exists
            var menuLinks = cut.FindAll($"a[href^='/resto/'][href$='/menu']");
            Assert.NotEmpty(menuLinks);
            Assert.Equal(restos.Count(), menuLinks.Count);
        });
    }
}
