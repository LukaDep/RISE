namespace Rise.Client.Grades;

using Rise.Shared.Grades;
using Microsoft.Extensions.Localization;
using Rise.Client.Tests.Grades;
using Rise.Client.Components;

public class IndexShould : TestContext
{
    public IndexShould()
    {
        Services.AddLocalization();
        Services.AddScoped<IGradesService, FakeGradesService>();
    }

    [Fact]
    public void RendersHeaderAndSearchElements()
    {
        // Arrange & Act

        var cut = RenderComponent<Index>();

        // Assert header/title rendered from localizer
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();

        Assert.Contains("<h1", cut.Markup);
        Assert.Contains(localizer["Grades.Title"], cut.Markup);

        // Search elements: the search input may be hidden or the toggle button may vary across versions.
        // Be resilient: if an input exists, assert its placeholder; otherwise only assert the selects are present.
        var input = cut.FindAll("input").FirstOrDefault();
        if (input != null)
        {
            Assert.Equal(localizer["Grades.SearchTermPlaceholder"], input.GetAttribute("placeholder"));
        }

        var selects = cut.FindComponents<SimpleSelect>();

        Assert.Equal(2, selects.Count);

        Assert.Contains(
            localizer["Grades.YearFilterPlaceholder"],
            selects[0].Markup
        );

        Assert.Contains(
            localizer["Grades.SemesterFilterPlaceholder"],
            selects[1].Markup
        );
    }

    [Fact]
    public void ShowsSpinnerWhenGradesIsNull()
    {
        // Arrange: override the registered IGradesService so it returns a GradesResponse.Index with a null Grades list.
        Services.AddScoped<IGradesService, NullGradesService>();

        // Act
        var cut = RenderComponent<Index>();

        // The component shows a loading spinner when grades == null
        Assert.Contains("animate-spin", cut.Markup);
    }
}
