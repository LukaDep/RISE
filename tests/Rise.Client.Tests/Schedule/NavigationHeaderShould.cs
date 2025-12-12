using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Rise.Client.Schedule;
using Shouldly;
using Xunit;

namespace Rise.Client.Schedule;

public class NavigationHeaderShould : TestContext
{
    public NavigationHeaderShould()
    {
        Services.AddLocalization();
    }

    [Fact]
    public void RenderTitle()
    {
        var cut = RenderComponent<NavigationHeader>(parameters => parameters
            .Add(p => p.Title, "Week 10: 4 Mar – 10 Mar"));

        cut.Markup.ShouldContain("Week 10: 4 Mar – 10 Mar");
    }

    [Fact]
    public void RenderPreviousButton()
    {
        var cut = RenderComponent<NavigationHeader>(parameters => parameters
            .Add(p => p.Title, "Test Title")
            .Add(p => p.PreviousText, "Previous"));

        var previousButton = cut.Find("button:first-child");
        previousButton.ShouldNotBeNull();
    }

    [Fact]
    public void RenderNextButton()
    {
        var cut = RenderComponent<NavigationHeader>(parameters => parameters
            .Add(p => p.Title, "Test Title")
            .Add(p => p.NextText, "Next"));

        var nextButton = cut.Find("button:last-child");
        nextButton.ShouldNotBeNull();
    }

    [Fact]
    public void InvokeOnPrevious_WhenPreviousButtonClicked()
    {
        var previousClicked = false;
        var cut = RenderComponent<NavigationHeader>(parameters => parameters
            .Add(p => p.Title, "Test Title")
            .Add(p => p.OnPrevious, EventCallback.Factory.Create(this, () => previousClicked = true)));

        var previousButton = cut.Find("button:first-child");
        previousButton.Click();

        previousClicked.ShouldBeTrue();
    }

    [Fact]
    public void InvokeOnNext_WhenNextButtonClicked()
    {
        var nextClicked = false;
        var cut = RenderComponent<NavigationHeader>(parameters => parameters
            .Add(p => p.Title, "Test Title")
            .Add(p => p.OnNext, EventCallback.Factory.Create(this, () => nextClicked = true)));

        var nextButton = cut.Find("button:last-child");
        nextButton.Click();

        nextClicked.ShouldBeTrue();
    }

    [Fact]
    public void RenderWithCustomButtonTexts()
    {
        var cut = RenderComponent<NavigationHeader>(parameters => parameters
            .Add(p => p.Title, "Test Title")
            .Add(p => p.PreviousText, "Vorige")
            .Add(p => p.NextText, "Volgende"));

        var previousButton = cut.Find("button:first-child");
        previousButton.ShouldNotBeNull();

        var nextButton = cut.Find("button:last-child");
        nextButton.ShouldNotBeNull();
    }

    [Fact]
    public void RenderWithIcons()
    {
        var cut = RenderComponent<NavigationHeader>(parameters => parameters
            .Add(p => p.Title, "Test Title"));

        // Should render SVG icons
        cut.Markup.ShouldContain("svg", Case.Insensitive);
    }

    [Fact]
    public void CenterTitleText()
    {
        var cut = RenderComponent<NavigationHeader>(parameters => parameters
            .Add(p => p.Title, "Centered Title"));

        // Title should have centered styling
        cut.Markup.ShouldContain("text-center");
    }
}
