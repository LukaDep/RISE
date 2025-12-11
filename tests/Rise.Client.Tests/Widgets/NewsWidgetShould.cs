using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

using Rise.Client.Home.Widgets;
using Rise.Client.News;
using Rise.Shared.News;
using Ardalis.Result;
using Rise.Client;
using Rise.Shared.Common;

namespace Rise.Client.Tests.Widgets;

public class EmptyNewsService : INewsService
{
    public Task<Result<NewsResponse.Index>> GetIndexAsync(QueryRequest.DateRange request, CancellationToken ctx = default)
    {
        var wrapper = new NewsResponse.Index
        {
            News = new List<NewsDto.Index>() // Empty list instead of null
        };

        return Task.FromResult(Result.Success(wrapper));
    }

    public Task<Result<NewsResponse.Get>> GetByIdAsync(Guid id, CancellationToken ctx = default)
    {
        return Task.FromResult(Result<NewsResponse.Get>.NotFound($"News with id {id} not found."));
    }
}

public class NewsWidgetShould : TestContext
{
    public NewsWidgetShould()
    {
        Services.AddLocalization();
        Services.AddScoped<INewsService, FakeNewsService>();
    }


    [Fact]
    public void RenderLatestNewsWhenLoaded()
    {
        var cut = RenderComponent<NewsWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"));
        
        Assert.Contains("Guest lecture series", cut.Markup);
        Assert.Contains("tester4", cut.Markup);
        Assert.Contains("read more", cut.Markup);
    }

    [Fact]
    public void ShowRemoveButtonInEditMode()
    {
        var cut = RenderComponent<NewsWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, true));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"));
        var removeButton = cut.Find("button[title='Remove widget']");
        Assert.NotNull(removeButton);
    }

    [Fact]
    public void NavigateToNewsPageWhenMoreClicked()
    {
        var navManager = Services.GetRequiredService<NavigationManager>();
        var cut = RenderComponent<NewsWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"));
        var moreLink = cut.Find("span.text-blue-600");
        moreLink.Click();

        Assert.Contains("/news", navManager.Uri);
    }

    [Fact]
    public void NavigateToNewsArticleWhenReadMoreClicked()
    {
        var navManager = Services.GetRequiredService<NavigationManager>();
        var cut = RenderComponent<NewsWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        cut.WaitForState(() => cut.Markup.Contains("read more"));
        var readMoreLink = cut.Find("span.underline");
        readMoreLink.Click();

        Assert.Contains("/news/", navManager.Uri);
    }

    [Fact]
    public void DisplayNoNewsMessageWhenEmpty()
    {
        // Create a new test context for this test to avoid service conflicts
        using var ctx = new TestContext();
        ctx.Services.AddLocalization();
        ctx.Services.AddScoped<INewsService, EmptyNewsService>();
        
        var cut = ctx.RenderComponent<NewsWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"), timeout: TimeSpan.FromSeconds(5));
        var localizer = ctx.Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        
        Assert.Contains(localizer["Home.NoNews"], cut.Markup);
    }

    [Fact]
    public void NotShowRemoveButtonWhenNotInEditMode()
    {
        var cut = RenderComponent<NewsWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"));
        Assert.DoesNotContain("Remove widget", cut.Markup);
    }

    [Fact]
    public void DisplayPublishDateInCorrectFormat()
    {
        var cut = RenderComponent<NewsWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"));
        var expectedDate = DateTime.UtcNow.ToString("dd MMM yyyy");
        Assert.Contains(expectedDate, cut.Markup);
    }
}

