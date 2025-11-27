using Microsoft.AspNetCore.Components;
using Rise.Shared.Resto;

namespace Rise.Client.Resto;

public class IndexSearchShould : TestContext
{
    private readonly NavigationManager navigation;

    public IndexSearchShould()
    {
        Services.AddLocalization();
        Services.AddScoped<IRestoService, FakeRestoService>();
        navigation = Services.GetRequiredService<NavigationManager>();
    }

    [Fact]
    public void SearchTermFiltersByName()
    {
        var uri = navigation.GetUriWithQueryParameter("SearchTerm", "Campus");
        navigation.NavigateTo(uri);
        var cut = RenderComponent<Index>();

        Assert.Contains("Campus Cafe", cut.Markup);
        Assert.DoesNotContain("Library Bistro", cut.Markup);
    }

    [Fact]
    public void SearchTermFiltersByDescription()
    {
        var uri = navigation.GetUriWithQueryParameter("SearchTerm", "salads");
        navigation.NavigateTo(uri);
        var cut = RenderComponent<Index>();

        Assert.Contains("Library Bistro", cut.Markup);
        Assert.DoesNotContain("Campus Cafe", cut.Markup);
    }

    [Fact]
    public void TypingInInputTriggersFuzzyFiltering()
    {
        var cut = RenderComponent<Index>();
        var input = cut.Find("input");
        input.Input("Campus");
        Assert.Contains("Campus Cafe", cut.Markup);
        Assert.DoesNotContain("Tech Lounge", cut.Markup);
    }
}
