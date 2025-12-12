using Rise.Client.Home.Widgets;

namespace Rise.Client.Tests.Widgets;

/// <summary>
/// Unit tests for the <see cref="LinksWidget"/> component.
/// </summary>
public class LinksWidgetShould : TestContext
{
    public LinksWidgetShould()
    {
        Services.AddLocalization();
    }

    [Fact]
    public void RenderAllLinks()
    {
        var cut = RenderComponent<LinksWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        Assert.Contains("ibamaflex.hogent.be", cut.Markup);
        Assert.Contains("wallie.hogent.be", cut.Markup);
        Assert.Contains("outlook.com/owa/hogent.be", cut.Markup);
        Assert.Contains("epurse.hogent.be", cut.Markup);
    }

    [Fact]
    public void ShowRemoveButtonInEditMode()
    {
        var cut = RenderComponent<LinksWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, true));

        var removeButton = cut.Find("button[title='Remove widget']");
        Assert.NotNull(removeButton);
    }

    [Fact]
    public void NotShowRemoveButtonWhenNotInEditMode()
    {
        var cut = RenderComponent<LinksWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        Assert.DoesNotContain("Remove widget", cut.Markup);
    }

    [Fact]
    public void RenderIconsCorrectly()
    {
        var cut = RenderComponent<LinksWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        // Icons used in the current component
        Assert.Contains("fa-bullhorn", cut.Markup);
        Assert.Contains("fa-envelope", cut.Markup);
        Assert.Contains("fa-credit-card", cut.Markup);
        Assert.Contains("fa-calendar-days", cut.Markup);
    }

    [Fact]
    public void RenderLinksAsAnchors()
    {
        var cut = RenderComponent<LinksWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        var links = cut.FindAll("a");
        // Component has 5 links: ibamaflex, wallie, outlook, epurse, sharepoint
        Assert.Equal(5, links.Count);
    }

    [Fact]
    public void LinksHaveCorrectStyling()
    {
        var cut = RenderComponent<LinksWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        var links = cut.FindAll("a");
        foreach (var link in links)
        {
            Assert.Contains("rounded-2xl", link.ClassList);
            Assert.Contains("bg-hogent-black", link.ClassList);
            Assert.Contains("text-hogent-white", link.ClassList);
        }
    }
}

