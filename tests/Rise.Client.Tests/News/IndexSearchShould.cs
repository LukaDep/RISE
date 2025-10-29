using Microsoft.AspNetCore.Components;
using Rise.Shared.News;
using Xunit.Abstractions;

namespace Rise.Client.News;

public class IndexSearchShould : TestContext
{
    private readonly NavigationManager NavigationManager;
    public IndexSearchShould()
    {
        Services.AddLocalization();
        Services.AddScoped<INewsService, FakeNewsService>();
        NavigationManager = Services.GetRequiredService<NavigationManager>();
    }
    [Fact]
    public void SearchTermFiltersByTitle()
    {
        var uri = NavigationManager.GetUriWithQueryParameter("SearchTerm", "Campus");
        NavigationManager.NavigateTo(uri);
        var cut = RenderComponent<Index>();

        // Assert that the matching title is shown and a non-matching title isn't
        Assert.Contains("Campus reopens", cut.Markup);
        Assert.DoesNotContain("New library hours", cut.Markup);
    }

    [Fact]
    public void SearchTermFiltersByContent()
    {
        var uri = NavigationManager.GetUriWithQueryParameter("SearchTerm", "vegetarian");
        NavigationManager.NavigateTo(uri);
        var cut = RenderComponent<Index>();

        // Assert that the matching item is shown
        Assert.Contains("Cafeteria menu updated", cut.Markup);
        // Ensure other items that don't contain the term are not present
        Assert.DoesNotContain("Guest lecture series", cut.Markup);
    }

    [Fact]
    public void TypingInInputNavigatesWithQueryParameter()
    {
        // Arrange
        var cut = RenderComponent<Index>();

        // Act: find the input and change its value to simulate typing
        var input = cut.Find("input");
        input.Input("Campus");

        // Assert navigation occurred with query parameter (component uses "searchTerm" as the query key when navigating)
        Assert.Contains("searchTerm=Campus", NavigationManager.Uri, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Campus reopens", cut.Markup);
        Assert.DoesNotContain("New library hours", cut.Markup);
    }
}
