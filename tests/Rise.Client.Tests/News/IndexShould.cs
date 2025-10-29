using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Rise.Shared.Common;
using Rise.Shared.News;
using Serilog.Core;
using Xunit.Abstractions;

namespace Rise.Client.News;

public class IndexShould : TestContext
{
    public IndexShould()
    {
        Services.AddLocalization();
        Services.AddScoped<INewsService, FakeNewsService>();
    }

    [Fact]
    public void RendersHeaderAndSearchElements()
    {
        // Arrange & Act

        var cut = RenderComponent<Index>();

        // Assert header/title rendered from localizer
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains("<h3", cut.Markup);
        Assert.Contains(localizer["News.Title"], cut.Markup);

        Assert.Contains($"placeholder=\"{localizer["News.FilterPlaceholder"]}\"", cut.Markup);

        // Assert the search button/svg exists
        Assert.Contains("<svg", cut.Markup);
    }

    [Fact]
    public void ShowsSpinnerWhenNewsIsNull()
    {
        // Arrange: override the registered INewsService so it returns a NewsResponse.Index with a null News list.
        Services.AddScoped<INewsService, NullNewsService>();

        // Act
        var cut = RenderComponent<Index>();

        // The component shows a loading spinner when news == null
        Assert.Contains("animate-spin", cut.Markup);
    }
}