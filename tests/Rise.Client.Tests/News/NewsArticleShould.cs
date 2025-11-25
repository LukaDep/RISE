using Rise.Shared.News;

namespace Rise.Client.News;

public class NewsArticleShould : TestContext
{
    private readonly FakeNewsService _fakeNewsService = new();

    public NewsArticleShould()
    {
        Services.AddLocalization();
        Services.AddScoped<INewsService>(_ => _fakeNewsService);
    }

    [Fact]
    public void RendersNewsArticleDetails()
    {
        // Arrange: get the first news item's ID from the fake service
        var newsId = _fakeNewsService.GetFirstNewsItemId();

        // Act: render the article for the existing ID
        var cut = RenderComponent<NewsArticle>(parameters => parameters.Add(p => p.Id, newsId));

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
        // Arrange: use a random non-existent GUID
        var nonExistentId = Guid.CreateVersion7();

        // Act: render the article for a non-existent Id
        var cut = RenderComponent<NewsArticle>(parameters => parameters.Add(p => p.Id, nonExistentId));

        // Assert that an error message is displayed
        Assert.Contains($"News item with id {nonExistentId} not found.", cut.Markup);
        // And that it's styled as an error (red text)
        Assert.Contains("text-red-500", cut.Markup);
    }
}