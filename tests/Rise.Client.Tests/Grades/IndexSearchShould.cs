namespace Rise.Client.Grades;

using Rise.Shared.Grades;
using Rise.Client.Tests.Grades;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Components;
using Rise.Client.Components;
using AngleSharp.Dom;

public class IndexSearchShould : TestContext
{
    private readonly NavigationManager NavigationManager;

    public IndexSearchShould()
    {
        Services.AddLocalization();
        Services.AddScoped<IGradesService, FakeGradesService>();
        NavigationManager = Services.GetRequiredService<NavigationManager>();

    }

    [Fact]
    public void RendersSearchButton()
    {
        // Arrange & Act
        var cut = RenderComponent<Index>();

        // Assert the search input exists
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        // placeholder text may vary with localization; require that either the placeholder
        // is present in the markup or a toggle button exists so test is deterministic.
        var hasPlaceholderText = cut.Markup.Contains(localizer["Grades.SearchTermPlaceholder"]);

        // find the toggle button by looking for the search icon inside a button (robust to attribute changes)
        var searchButton = cut.FindAll("button").FirstOrDefault(b => b.OuterHtml.Contains("fa-magnifying-glass") || b.OuterHtml.Contains("fa-xmark"));

        // Ensure at least one of the UI affordances exists (placeholder or toggle)
        Assert.True(hasPlaceholderText || searchButton is not null, "Neither the search placeholder nor the search toggle were rendered.");

        // If the placeholder text is present we expect the input to already be rendered.
        if (hasPlaceholderText)
        {
            var input = cut.Find("input");
            Assert.Equal(localizer["Common.SearchPlaceholder"], input.GetAttribute("placeholder"));
            return;
        }

        // Otherwise, the toggle must exist and open the input when clicked.
        Assert.NotNull(searchButton);
        Assert.Empty(cut.FindAll("input"));

        // click to open the input
        searchButton!.Click();

        // input should be rendered and have the common placeholder
        var inputAfter = cut.Find("input");
        Assert.Equal(localizer["Common.SearchPlaceholder"], inputAfter.GetAttribute("placeholder"));
    }

    [Fact]
    public void SearchByCourseName()
    {
        var uri = NavigationManager.GetUriWithQueryParameter("SearchTerm", "Math");
        NavigationManager.NavigateTo(uri);

        var cut = RenderComponent<Index>();

        // Assert that the matching course is shown and a non-matching course isn't
        Assert.Contains("Mathematics", cut.Markup);
        Assert.DoesNotContain("Chemistry", cut.Markup);
    }

    [Fact]
    public void SearchByGradeName()
    {
        // Arrange
        var cut = RenderComponent<Index>();

        // Act: Open the search input by clicking the search button
        var searchButton = cut.FindAll("button").FirstOrDefault(b => b.OuterHtml.Contains("fa-magnifying-glass") || b.OuterHtml.Contains("fa-xmark"));
        Assert.NotNull(searchButton);
        searchButton.Click();

        // Find the input and change its value to simulate typing
        var input = cut.Find("input");
        input.Input("Final");

        // Assert that the matching grade is shown and a non-matching grade isn't
        Assert.Contains("Final Exam", cut.Markup);
        Assert.DoesNotContain("Midterm Exam", cut.Markup);
    }

    [Fact]
    public void YearFilter()
    {
        var cut = RenderComponent<Index>();

        var selects = cut.FindComponents<SimpleSelect>();
        selects[0].Find("button").Click();
        var yearOption = selects[0].FindAll("div[role='option']")
            .FirstOrDefault(o => o.TextContent.Contains("2024"));
        Assert.NotNull(yearOption);
        yearOption.Click();


        Assert.DoesNotContain("Midterm Exam", cut.Markup);
        Assert.DoesNotContain("Final Exam", cut.Markup);
        Assert.DoesNotContain("Project", cut.Markup);
        Assert.Contains("Lab Work", cut.Markup);
    }

    [Fact]
    public void SemesterFilter()
    {
        var cut = RenderComponent<Index>();

        var selects = cut.FindComponents<SimpleSelect>();
        var semesterSelect = selects[1];
        semesterSelect.Find("button").Click();

        // alle vakken zijn semester 1
        var semester1Option = semesterSelect.FindAll("div[role='option']")
            .FirstOrDefault(o => o.TextContent.Contains("1"));
        Assert.NotNull(semester1Option);
        semester1Option.Click();

        Assert.Contains("Midterm Exam", cut.Markup);
        Assert.Contains("Final Exam", cut.Markup);
        Assert.DoesNotContain("Lab Work", cut.Markup);
        Assert.Contains("Project", cut.Markup);

        // Now select semester 2 where no grades exist
        // open the dropdown again (it closes after selecting an option)
        semesterSelect.Find("button").Click();

        var semester2Option = semesterSelect.FindAll("div[role='option']")
            .FirstOrDefault(o => o.TextContent.Contains("2"));
        Assert.NotNull(semester2Option);
        semester2Option.Click();

        Assert.DoesNotContain("Midterm Exam", cut.Markup);
        Assert.DoesNotContain("Final Exam", cut.Markup);
        Assert.Contains("Lab Work", cut.Markup);
        Assert.DoesNotContain("Project", cut.Markup);

    }
}
