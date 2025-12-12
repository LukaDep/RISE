using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Rise.Client;
using Rise.Client.Home;
using Rise.Client.Schedule;
using Rise.Client.Resto;
using Rise.Client.Menu;
using Rise.Client.News;
using Rise.Shared.Widgets;
using Rise.Shared.Schedule;
using Rise.Shared.Resto;
using Rise.Shared.Menu;
using Rise.Shared.News;
using Rise.Shared.Grades;
using Index = Rise.Client.Home.Index;

namespace Rise.Client.Tests.Widgets;

/// <summary>
/// Unit tests for the Home <see cref="Index"/> page component.
/// </summary>
public class IndexShould : TestContext
{
    public IndexShould()
    {
        Services.AddLocalization();
        Services.AddScoped<IWidgetService, FakeWidgetService>();
        Services.AddScoped<IScheduleService, FakeScheduleService>();
        Services.AddScoped<IRestoService, FakeRestoService>();
        Services.AddScoped<IMenuService, FakeMenuService>();
        Services.AddScoped<INewsService, FakeNewsService>();
        Services.AddScoped<IGradesService, FakeGradesService>();
        
        // Mock JavaScript interop for gridstack
        JSInterop.SetupVoid("gridstackInterop.initGrid", _ => true);
        JSInterop.SetupVoid("gridstackInterop.setEditMode", _ => true);
        JSInterop.SetupVoid("gridstackInterop.destroy", _ => true);
        JSInterop.Setup<List<WidgetEntry>>("gridstackInterop.getWidgets")
            .SetResult(new List<WidgetEntry>());
    }
    
    [Fact]
    public void RendersHeaderAndWidgets()
    {
        var authContext = this.AddTestAuthorization();
        authContext.SetNotAuthorized();
        
        var cut = RenderComponent<Index>();
        Assert.Contains("<img", cut.Markup);
    }

    [Fact]
    public void RendersHogentLogo()
    {
        var authContext = this.AddTestAuthorization();
        authContext.SetNotAuthorized();
        
        var cut = RenderComponent<Index>();
        Assert.Contains("/HOGENT-Logo-Home.png", cut.Markup);
        Assert.Contains("Hogent Logo", cut.Markup);
    }

    [Fact]
    public void ShowsEditButtonWhenAuthenticated()
    {
        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("TestUser");

        var cut = RenderComponent<Index>();
        
        // Edit button uses localized label
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        var editButton = cut.Find($"button[aria-label='{localizer["Home.Edit"]}']");
        Assert.NotNull(editButton);
        Assert.Contains("fa-pencil", editButton.InnerHtml);
    }

    [Fact]
    public void DoesNotShowEditButtonWhenNotAuthenticated()
    {
        var authContext = this.AddTestAuthorization();
        authContext.SetNotAuthorized();

        var cut = RenderComponent<Index>();
        
        // Edit button uses localized label
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        var editButtons = cut.FindAll($"button[aria-label='{localizer["Home.Edit"]}']");
        Assert.Empty(editButtons);
    }

    [Fact]
    public void EntersEditModeWhenEditButtonClicked()
    {
        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("TestUser");

        var cut = RenderComponent<Index>();
        var navManager = Services.GetRequiredService<NavigationManager>();
        
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        var editButton = cut.Find($"button[aria-label='{localizer["Home.Edit"]}']");
        editButton.Click();

        Assert.Contains("/edit", navManager.Uri);
    }

    [Fact]
    public void ShowsSaveButtonInEditMode()
    {
        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("TestUser");

        var cut = RenderComponent<Index>(parameters => parameters
            .Add(p => p.Mode, "edit"));
        
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        var saveButton = cut.Find($"button[aria-label='{localizer["Home.Save"]}']");
        Assert.NotNull(saveButton);
        Assert.Contains("fa-check", saveButton.InnerHtml);
    }

    [Fact]
    public void ShowsAddWidgetButtonInEditMode()
    {
        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("TestUser");

        var cut = RenderComponent<Index>(parameters => parameters
            .Add(p => p.Mode, "edit"));
        
        var addButton = cut.Find("button[aria-label='Add widget']");
        Assert.NotNull(addButton);
        Assert.Contains("fa-plus", addButton.InnerHtml);
    }

    [Fact]
    public void DoesNotShowAddWidgetButtonWhenNotInEditMode()
    {
        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("TestUser");

        var cut = RenderComponent<Index>();
        
        var addButtons = cut.FindAll("button[aria-label='Add widget']");
        Assert.Empty(addButtons);
    }

    [Fact]
    public void ExitsEditModeWhenSaveButtonClicked()
    {
        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("TestUser");

        var cut = RenderComponent<Index>(parameters => parameters
            .Add(p => p.Mode, "edit"));
        var navManager = Services.GetRequiredService<NavigationManager>();
        
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        var saveButton = cut.Find($"button[aria-label='{localizer["Home.Save"]}']");
        saveButton.Click();

        Assert.DoesNotContain("edit", navManager.Uri);
    }
    
}