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
        Assert.Contains("Campus reopens", cut.Markup);
        Assert.DoesNotContain("New library hours", cut.Markup);
    }

    [Fact]
    public void SearchTermFiltersByContent()
    {
        var uri = NavigationManager.GetUriWithQueryParameter("SearchTerm", "vegetarian");
        NavigationManager.NavigateTo(uri);
        var cut = RenderComponent<Index>();
        Assert.Contains("Cafeteria menu updated", cut.Markup);
        Assert.DoesNotContain("Guest lecture series", cut.Markup);
    }

    [Fact]
    public void TypingInInputNavigatesWithQueryParameter()
    {
        var cut = RenderComponent<Index>();
        var input = cut.Find("input");
        input.Input("Campus");
        Assert.Contains("searchTerm=Campus", NavigationManager.Uri, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Campus reopens", cut.Markup);
        Assert.DoesNotContain("New library hours", cut.Markup);
    }
}
