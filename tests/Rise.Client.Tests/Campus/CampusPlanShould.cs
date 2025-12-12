using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Rise.Client.Campus;
using Rise.Shared.Campus;
using Shouldly;
using Xunit;

namespace Rise.Client.Campus;

public class CampusPlanShould : TestContext
{
    private readonly FakeCampusService _fakeCampusService = new();

    public CampusPlanShould()
    {
        Services.AddScoped<ICampusService>(_ => _fakeCampusService);
        Services.AddLocalization();

        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void RenderMapContainerWhenCampusLoaded()
    {
        var campusId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var navManager = Services.GetRequiredService<NavigationManager>();
        var uri = navManager.GetUriWithQueryParameter("returnUrl", "campus");
        navManager.NavigateTo(uri);
        
        var cut = RenderComponent<CampusPlan>(parameters => parameters
            .Add(p => p.campusId, campusId));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        cut.Markup.ShouldContain("id=\"map\"");
        cut.Markup.ShouldContain("h-[100vh]");
    }

    [Fact]
    public void RenderBackButton()
    {
        var campusId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var navManager = Services.GetRequiredService<NavigationManager>();
        var uri = navManager.GetUriWithQueryParameter("returnUrl", "campus");
        navManager.NavigateTo(uri);
        
        var cut = RenderComponent<CampusPlan>(parameters => parameters
            .Add(p => p.campusId, campusId));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // BackButton renders as NavLink (a tag), not button
        var backLink = cut.Find("a");
        backLink.ShouldNotBeNull();
        cut.Markup.ShouldContain("fa-arrow-left");
    }

    [Fact]
    public void InvokeGoBackOnButtonClick()
    {
        var campusId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var navManager = Services.GetRequiredService<NavigationManager>();
        var uri = navManager.GetUriWithQueryParameter("returnUrl", "/campus");
        navManager.NavigateTo(uri);
        
        var cut = RenderComponent<CampusPlan>(parameters => parameters
            .Add(p => p.campusId, campusId));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        // BackButton renders as NavLink (a tag) with href="/campus"
        var backLink = cut.Find("a");
        backLink.ShouldNotBeNull();
        backLink.GetAttribute("href").ShouldBe("/campus");
    }

    [Fact]
    public void InitializeLeafletMapAfterRender()
    {
        var campusId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var navManager = Services.GetRequiredService<NavigationManager>();
        var uri = navManager.GetUriWithQueryParameter("returnUrl", "campus");
        navManager.NavigateTo(uri);

        var cut = RenderComponent<CampusPlan>(parameters => parameters
            .Add(p => p.campusId, campusId));

        cut.WaitForState(() => JSInterop.Invocations.Any(inv => inv.Identifier == "leafletMap.initTileMap"), TimeSpan.FromSeconds(3));

        JSInterop.Invocations.ShouldContain(inv => inv.Identifier == "leafletMap.initTileMap");
    }

    [Fact]
    public void AddMarkerForCampusLocation()
    {
        var campusId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var navManager = Services.GetRequiredService<NavigationManager>();
        var uri = navManager.GetUriWithQueryParameter("returnUrl", "campus");
        navManager.NavigateTo(uri);

        var cut = RenderComponent<CampusPlan>(parameters => parameters
            .Add(p => p.campusId, campusId));

        cut.WaitForState(() => JSInterop.Invocations.Any(inv => inv.Identifier == "leafletMap.addMarkerWithGoogleLink"), TimeSpan.FromSeconds(3));

        JSInterop.Invocations.ShouldContain(inv => inv.Identifier == "leafletMap.addMarkerWithGoogleLink");
    }

    [Fact]
    public void HandleBuildingCodeParameter()
    {
        var campusId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var buildingCode = "A";
        var navManager = Services.GetRequiredService<NavigationManager>();
        var uri = navManager.GetUriWithQueryParameter("returnUrl", "campus");
        navManager.NavigateTo(uri);

        var cut = RenderComponent<CampusPlan>(parameters => parameters
            .Add(p => p.campusId, campusId)
            .Add(p => p.buildingCode, buildingCode));

        cut.WaitForState(() => !string.IsNullOrEmpty(cut.Markup), TimeSpan.FromSeconds(2));

        cut.Markup.ShouldContain("id=\"map\"");
    }
}
