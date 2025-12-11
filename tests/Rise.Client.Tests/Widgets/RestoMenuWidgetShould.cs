using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Rise.Client;
using Rise.Client.Home.Widgets;
using Rise.Client.Resto;
using Rise.Client.Menu;
using Rise.Shared.Resto;
using Rise.Shared.Menu;

namespace Rise.Client.Tests.Widgets;

public class RestoMenuWidgetShould : TestContext
{
    public RestoMenuWidgetShould()
    {
        Services.AddLocalization();
        Services.AddScoped<IRestoService, FakeRestoService>();
        Services.AddScoped<IMenuService, FakeMenuService>();
    }


    [Fact]
    public void RenderRestoNameWhenLoaded()
    {
        var cut = RenderComponent<RestoMenuWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"), timeout: TimeSpan.FromSeconds(5));
        
        Assert.Contains("Campus Cafe", cut.Markup);
    }

    [Fact]
    public void ShowRemoveButtonInEditMode()
    {
        var cut = RenderComponent<RestoMenuWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, true));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"), timeout: TimeSpan.FromSeconds(5));
        var removeButton = cut.Find("button[title='Remove widget']");
        Assert.NotNull(removeButton);
    }

    [Fact]
    public void ShowMoreLinkWhenNotInEditMode()
    {
        var cut = RenderComponent<RestoMenuWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"), timeout: TimeSpan.FromSeconds(5));
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains(localizer["Home.More"], cut.Markup);
    }

    [Fact]
    public void NavigateToRestoPageWhenMoreClicked()
    {
        var navManager = Services.GetRequiredService<NavigationManager>();
        var cut = RenderComponent<RestoMenuWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"), timeout: TimeSpan.FromSeconds(5));
        var moreLink = cut.Find("span.text-blue-600");
        moreLink.Click();

        Assert.Contains("/resto", navManager.Uri);
    }

    [Fact]
    public void ShowNavigationButtons()
    {
        var cut = RenderComponent<RestoMenuWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"), timeout: TimeSpan.FromSeconds(5));
        var prevButton = cut.Find("button[aria-label='Previous']");
        var nextButton = cut.Find("button[aria-label='Next']");
        
        Assert.NotNull(prevButton);
        Assert.NotNull(nextButton);
    }

    [Fact]
    public void NotShowRemoveButtonWhenNotInEditMode()
    {
        var cut = RenderComponent<RestoMenuWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"), timeout: TimeSpan.FromSeconds(5));
        Assert.DoesNotContain("Remove widget", cut.Markup);
    }

    [Fact]
    public void DisplayOpenStatusWhenOpen()
    {
        var cut = RenderComponent<RestoMenuWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"), timeout: TimeSpan.FromSeconds(5));
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        // Campus Cafe is set to IsCurrentlyOpen = true in FakeRestoService
        Assert.Contains(localizer["Resto.Open"], cut.Markup);
    }
}

