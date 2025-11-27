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

        var cut = RenderComponent<Index>();
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains("<h3", cut.Markup);
        Assert.Contains(localizer["News.Title"], cut.Markup);

        Assert.Contains($"placeholder=\"{localizer["News.FilterPlaceholder"]}\"", cut.Markup);
        Assert.Contains("<svg", cut.Markup);
    }

    [Fact]
    public void ShowsSpinnerWhenNewsIsNull()
    {
        Services.AddScoped<INewsService, NullNewsService>();
        var cut = RenderComponent<Index>();
        Assert.Contains("animate-spin", cut.Markup);
    }
}