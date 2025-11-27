/*
using Xunit.Abstractions;

namespace Rise.Client.Pages;
public class IndexShould : TestContext
{
    public IndexShould(ITestOutputHelper outputHelper)
    {
        Services.AddXunitLogger(outputHelper);
        Services.AddLocalization();
    }

    [Fact]
    public void RendersLocalizedComingSoonHeader()
    {
        var cut = RenderComponent<Index>();
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains($"<h1>{localizer["Common.ComingSoon"]}</h1>", cut.Markup);
    }
}

*/