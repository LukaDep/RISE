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
    var existingId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    var cut = RenderComponent<NewsArticle>(parameters =>
        parameters.Add(p => p.Id, existingId));

    Assert.Contains("Campus reopens", cut.Markup);
    Assert.Contains("We are happy to announce the campus reopens.", cut.Markup);
    Assert.Contains("Admin", cut.Markup);

    var img = cut.Find("img");
    Assert.Equal("Campus reopens", img.GetAttribute("alt"));

    var backLink = cut.FindAll("a[href='/news']");
    Assert.NotEmpty(backLink);
}


    [Fact]
public void NonExistentIdShowsErrorMessage()
{
    // A GUID guaranteed not to exist in FakeNewsService
    var nonExistingId = new Guid("99999999-9999-9999-9999-999999999999");

    // Render the component
    var cut = RenderComponent<NewsArticle>(parameters =>
        parameters.Add(p => p.Id, nonExistingId));

    // Verify the error message
    Assert.Contains($"News item with id {nonExistingId} not found.", cut.Markup);

    // Verify error styling
    Assert.Contains("text-red-500", cut.Markup);
}

}
