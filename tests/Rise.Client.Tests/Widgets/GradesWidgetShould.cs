using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Rise.Client;
using Rise.Client.Home.Widgets;
using Rise.Shared.Grades;

namespace Rise.Client.Tests.Widgets;

public class GradesWidgetShould : TestContext
{
    public GradesWidgetShould()
    {
        Services.AddLocalization();
        Services.AddScoped<IGradesService, FakeGradesService>();
    }


    [Fact]
    public void RenderGradeDetailsWhenLoaded()
    {
        var cut = RenderComponent<GradesWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"));
        
        Assert.Contains("Mathematics", cut.Markup);
        Assert.Contains("15", cut.Markup);
        Assert.Contains("20", cut.Markup);
    }

    [Fact]
    public void ShowRemoveButtonInEditMode()
    {
        var cut = RenderComponent<GradesWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, true));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"));
        var removeButton = cut.Find("button[title='Remove widget']");
        Assert.NotNull(removeButton);
        Assert.Contains("×", removeButton.TextContent);
    }

    [Fact]
    public void ShowMoreLinkWhenNotInEditMode()
    {
        var cut = RenderComponent<GradesWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"));
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains(localizer["Home.More"], cut.Markup);
    }

    [Fact]
    public void NavigateToGradesPageWhenMoreClicked()
    {
        var navManager = Services.GetRequiredService<NavigationManager>();
        var cut = RenderComponent<GradesWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"));
        // More button is now a button element with text-hogent-education class
        var moreButton = cut.FindAll("button").First(b => b.ClassList.Contains("text-hogent-education"));
        moreButton.Click();

        Assert.Contains("/grades", navManager.Uri);
    }

    [Fact]
    public void DisplayNoGradesMessageWhenEmpty()
    {
        // Create a new test context for this test to avoid service conflicts
        using var ctx = new TestContext();
        ctx.Services.AddLocalization();
        ctx.Services.AddScoped<IGradesService, NullGradesService>();
        
        var cut = ctx.RenderComponent<GradesWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"), timeout: TimeSpan.FromSeconds(5));
        var localizer = ctx.Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        
        Assert.Contains(localizer["Home.NoGrades"], cut.Markup);
    }

    [Fact]
    public void NotShowRemoveButtonWhenNotInEditMode()
    {
        var cut = RenderComponent<GradesWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"));
        Assert.DoesNotContain("Remove widget", cut.Markup);
    }

    [Fact]
    public void DisplayGradeInfoCorrectly()
    {
        var cut = RenderComponent<GradesWidget>(parameters => parameters
            .Add(p => p.WidgetId, Guid.NewGuid())
            .Add(p => p.EditMode, false));

        cut.WaitForState(() => !cut.Markup.Contains("Loading…"));
        // The GradesWidget displays the latest grade info, not a date
        Assert.Contains("Mathematics", cut.Markup);
    }
}

