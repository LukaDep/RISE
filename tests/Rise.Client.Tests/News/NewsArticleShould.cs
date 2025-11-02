using Microsoft.AspNetCore.Components;
using Rise.Shared.News;
using Xunit.Abstractions;

namespace Rise.Client.News;

public class NewsArticleShould : TestContext
{
    public NewsArticleShould()
    {
        Services.AddLocalization();
        Services.AddScoped<INewsService, FakeNewsService>();
    }

    [Fact]
    public void RendersNewsArticleDetails()
    {
        // Arrange & Act: render the article for Id=1 (exists in FakeNewsService)
        var cut = RenderComponent<NewsArticle>(parameters => parameters.Add(p => p.Id, "1"));

        // Assert title and content are present
        Assert.Contains("Campus reopens", cut.Markup);
        Assert.Contains("We are happy to announce the campus reopens.", cut.Markup);

        // Assert author is present
        Assert.Contains("Admin", cut.Markup);

        // Assert image alt uses the title
        var img = cut.Find("img");
        Assert.Equal("Campus reopens", img.GetAttribute("alt"));

        // Assert there's a back link to the news overview
        var backLink = cut.FindAll("a[href='/news']");
        Assert.NotEmpty(backLink);
    }

    [Fact]
    public void NonExistentIdShowsErrorMessage()
    {
        // Arrange & Act: render the article for a non-existent Id (999)
        var cut = RenderComponent<NewsArticle>(parameters => parameters.Add(p => p.Id, "999"));

        // Assert that an error message is displayed
        Assert.Contains("News item with id 999 not found.", cut.Markup);
        // And that it's styled as an error (red text)
        Assert.Contains("text-red-500", cut.Markup);
    }
}
