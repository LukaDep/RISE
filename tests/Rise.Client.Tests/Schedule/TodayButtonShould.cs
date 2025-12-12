using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Rise.Client.Schedule;
using Shouldly;
using Xunit;

namespace Rise.Client.Schedule;

public class TodayButtonShould : TestContext
{
    public TodayButtonShould()
    {
        Services.AddLocalization();
    }

    [Fact]
    public void RenderButton_WhenShowButtonIsTrue()
    {
        var cut = RenderComponent<TodayButton>(parameters => parameters
            .Add(p => p.ShowButton, true)
            .Add(p => p.ButtonText, "Today"));

        var button = cut.Find("button");
        button.ShouldNotBeNull();
        button.TextContent.ShouldContain("Today");
    }

    [Fact]
    public void NotRenderButton_WhenShowButtonIsFalse()
    {
        var cut = RenderComponent<TodayButton>(parameters => parameters
            .Add(p => p.ShowButton, false));

        cut.Markup.Trim().ShouldBeEmpty();
    }

    [Fact]
    public void InvokeOnClick_WhenButtonClicked()
    {
        var clicked = false;
        var cut = RenderComponent<TodayButton>(parameters => parameters
            .Add(p => p.ShowButton, true)
            .Add(p => p.ButtonText, "Today")
            .Add(p => p.OnClick, EventCallback.Factory.Create(this, () => clicked = true)));

        var button = cut.Find("button");
        button.Click();

        clicked.ShouldBeTrue();
    }

    [Fact]
    public void RenderCustomButtonText()
    {
        var cut = RenderComponent<TodayButton>(parameters => parameters
            .Add(p => p.ShowButton, true)
            .Add(p => p.ButtonText, "Go to Today"));

        var button = cut.Find("button");
        button.TextContent.ShouldContain("Go to Today");
    }

    [Fact]
    public void RenderLocalizedButtonText()
    {
        var cut = RenderComponent<TodayButton>(parameters => parameters
            .Add(p => p.ShowButton, true)
            .Add(p => p.ButtonText, "Vandaag"));

        var button = cut.Find("button");
        button.TextContent.ShouldContain("Vandaag");
    }

    [Fact]
    public void ApplyButtonStyling()
    {
        var cut = RenderComponent<TodayButton>(parameters => parameters
            .Add(p => p.ShowButton, true)
            .Add(p => p.ButtonText, "Today"));

        var button = cut.Find("button");
        button.ClassName.ShouldNotBeEmpty();
    }
}
