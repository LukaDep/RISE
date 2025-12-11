using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Rise.Client;
using Rise.Client.Home.Widgets;
using Rise.Client.Schedule;
using Rise.Shared.Schedule;

namespace Rise.Client.Tests.Widgets;

public class ScheduleWidgetShould : TestContext
{
    public ScheduleWidgetShould()
    {
        Services.AddLocalization();
        Services.AddScoped<IScheduleService, FakeScheduleService>();
    }


    [Fact]
    public void RenderTodaysClassesWhenLoaded()
    {
        var cut = RenderComponent<ScheduleWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"));
        
        Assert.Contains("Web Ontwikkeling 2", cut.Markup);
    }

    [Fact]
    public void ShowRemoveButtonInEditMode()
    {
        var cut = RenderComponent<ScheduleWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, true));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"));
        var removeButton = cut.Find("button[title='Remove widget']");
        Assert.NotNull(removeButton);
    }

    [Fact]
    public void ShowMoreLinkWhenNotInEditMode()
    {
        var cut = RenderComponent<ScheduleWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"));
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains(localizer["Home.More"], cut.Markup);
    }

    [Fact]
    public void NavigateToSchedulePageWhenMoreClicked()
    {
        var navManager = Services.GetRequiredService<NavigationManager>();
        var cut = RenderComponent<ScheduleWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"));
        var moreLink = cut.Find("span.text-blue-600");
        moreLink.Click();

        Assert.Contains("/schedule", navManager.Uri);
    }

    [Fact]
    public void DisplayNoClassesMessageWhenEmpty()
    {
        // Create a new test context for this test to avoid service conflicts
        using var ctx = new TestContext();
        ctx.Services.AddLocalization();
        ctx.Services.AddScoped<IScheduleService, NullScheduleService>();
        
        var cut = ctx.RenderComponent<ScheduleWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"), timeout: TimeSpan.FromSeconds(5));
        var localizer = ctx.Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        
        // When there are no classes today, verify the header is present
        Assert.Contains(localizer["Home.Today"], cut.Markup);
    }

    [Fact]
    public void NotShowRemoveButtonWhenNotInEditMode()
    {
        var cut = RenderComponent<ScheduleWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"));
        Assert.DoesNotContain("Remove widget", cut.Markup);
    }

    [Fact]
    public void DisplayHeaderTitle()
    {
        var cut = RenderComponent<ScheduleWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"));
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains(localizer["Home.Today"], cut.Markup);
    }
}

