using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Components;
using Rise.Shared.Menu;

namespace Rise.Client.Menu;

/// <summary>
/// Unit tests for the <see cref="MenuPage"/> component.
/// </summary>
public class MenuPageShould : TestContext
{
    private static readonly Guid Resto1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public MenuPageShould()
    {
        Services.AddLocalization();
        Services.AddScoped<IMenuService, FakeMenuService>();
    }

    [Fact]
    public void RenderHeaderAndBackButton()
    {
        // Arrange & Act
        var cut = RenderComponent<MenuPage>(parameters => parameters
            .Add(p => p.RestoId, Resto1Id));

        // Assert header title rendered
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains(localizer["Resto.MenuTitle"], cut.Markup);

        // Assert back button exists (BackButton uses fa-arrow-left)
        Assert.Contains("fa-arrow-left", cut.Markup);
    }

    [Fact]
    public void RenderFilterButtons()
    {
        // Arrange & Act
        var cut = RenderComponent<MenuPage>(parameters => parameters
            .Add(p => p.RestoId, Resto1Id));

        // Assert filter buttons exist
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();

        Assert.Contains(localizer["Resto.Filter.All"], cut.Markup);
        Assert.Contains(localizer["Resto.Vegetarisch"], cut.Markup);
        Assert.Contains(localizer["Resto.Vegan"], cut.Markup);

        // Check for filter icons
        Assert.Contains("fa-list", cut.Markup);
        Assert.Contains("fa-leaf", cut.Markup);
        Assert.Contains("fa-seedling", cut.Markup);
    }

    [Fact]
    public void ShowLegendWithIconExplanations()
    {
        // Arrange & Act
        var cut = RenderComponent<MenuPage>(parameters => parameters
            .Add(p => p.RestoId, Resto1Id));

        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();

        cut.WaitForAssertion(() =>
        {
            // Check for legend section
            Assert.Contains(localizer["Resto.Legende"], cut.Markup);

            // Verify legend contains icon explanations
            var legendSection = cut.Markup.Substring(cut.Markup.IndexOf(localizer["Resto.Legende"]));
            Assert.Contains(localizer["Resto.Vegetarisch"], legendSection);
            Assert.Contains(localizer["Resto.Vegan"], legendSection);
        });
    }

    [Fact]
    public void FilterVegetarianItemsWhenVeggieFilterSelected()
    {
        // Arrange
        var cut = RenderComponent<MenuPage>(parameters => parameters
            .Add(p => p.RestoId, Resto1Id));

        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();

        cut.WaitForAssertion(() =>
        {
            // Wait for filter buttons to be rendered and click veggie filter
            var buttons = cut.FindAll("button");
            var veggieButton = buttons.FirstOrDefault(b =>
                b.TextContent.Contains(localizer["Resto.Vegetarisch"]) &&
                b.ClassList.Contains("px-4"));
            Assert.NotNull(veggieButton);
            veggieButton.Click();

            // Assert - Veggie filter button should be active (green)
            var activeButton = cut.FindAll("button").FirstOrDefault(b =>
                b.TextContent.Contains(localizer["Resto.Vegetarisch"]) &&
                b.ClassList.Contains("bg-green-600"));
            Assert.NotNull(activeButton);

            // Veggie items should be visible (in expanded today section)
            var markup = cut.Markup;
            Assert.Contains("Vegetarian Lasagna", markup);
        });
    }

    [Fact]
    public void FilterVeganItemsWhenVeganFilterSelected()
    {
        // Arrange
        var cut = RenderComponent<MenuPage>(parameters => parameters
            .Add(p => p.RestoId, Resto1Id));

        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();

        cut.WaitForAssertion(() =>
        {
            // Wait for filter buttons to be rendered and click vegan filter
            var buttons = cut.FindAll("button");
            var veganButton = buttons.FirstOrDefault(b =>
                b.TextContent.Contains(localizer["Resto.Vegan"]) &&
                !b.TextContent.Contains(localizer["Resto.Vegetarisch"]) &&
                b.ClassList.Contains("px-4"));
            Assert.NotNull(veganButton);
            veganButton.Click();

            // Assert - Vegan filter button should be active (emerald)
            var activeButton = cut.FindAll("button").FirstOrDefault(b =>
                b.TextContent.Contains(localizer["Resto.Vegan"]) &&
                !b.TextContent.Contains(localizer["Resto.Vegetarisch"]) &&
                b.ClassList.Contains("bg-emerald-500"));
            Assert.NotNull(activeButton);

            // Vegan items should be visible (in expanded today section)
            var markup = cut.Markup;
            Assert.Contains("Tomato Soup", markup); // Tomato soup is vegan
        });
    }

    [Fact]
    public void ToggleDayExpansion()
    {
        // Arrange
        var cut = RenderComponent<MenuPage>(parameters => parameters
            .Add(p => p.RestoId, Resto1Id));

        cut.WaitForAssertion(() =>
        {
            // Assert - Today should be expanded by default (contains menu items)
            var initialMarkup = cut.Markup;
            var hasExpandedContent = initialMarkup.Contains("Spaghetti") ||
                                     initialMarkup.Contains("Vegetarian") ||
                                     initialMarkup.Contains("Tomato Soup");

            Assert.True(hasExpandedContent, "Expected today's menu to be expanded by default");

            // Act - Click to collapse/expand days
            var dayHeaders = cut.FindAll("button").Where(b =>
                b.ClassList.Contains("w-full") &&
                b.ClassList.Contains("text-left"));

            if (dayHeaders.Any())
            {
                var firstDay = dayHeaders.First();
                firstDay.Click();

                // The day should toggle (expand or collapse)
                var afterClickMarkup = cut.Markup;
                Assert.NotEqual(initialMarkup, afterClickMarkup);
            }
        });
    }

    [Fact]
    public void NavigateBackWhenBackButtonClicked()
    {
        // Arrange & Act
        var cut = RenderComponent<MenuPage>(parameters => parameters
            .Add(p => p.RestoId, Resto1Id));

        // Assert - Back link should exist with correct href
        var backLink = cut.FindAll("a")
            .FirstOrDefault(a => a.GetAttribute("href") == "/resto");
        
        Assert.NotNull(backLink);
        Assert.Contains("fa-arrow-left", backLink.InnerHtml);
    }

    [Fact]
    public void ShowOnlyWeekdayMenus()
    {
        // Arrange & Act
        var cut = RenderComponent<MenuPage>(parameters => parameters
            .Add(p => p.RestoId, Resto1Id));

        cut.WaitForAssertion(() =>
        {
            // Assert - All displayed days should be weekdays
            var markup = cut.Markup;

            // The page should show menus (if today is a weekday or it should show next Monday)
            // FakeMenuService provides menus for weekdays only
            Assert.DoesNotContain("Saturday", markup);
            Assert.DoesNotContain("Sunday", markup);
        });
    }

    [Fact]
    public void DisplayMenuItemsWithPrices()
    {
        // Arrange & Act
        var cut = RenderComponent<MenuPage>(parameters => parameters
            .Add(p => p.RestoId, Resto1Id));

        cut.WaitForAssertion(() =>
        {
            var markup = cut.Markup;

            // Assert - Should display menu items from FakeMenuService
            Assert.Contains("Spaghetti Bolognese", markup);
            Assert.Contains("Vegetarian Lasagna", markup);

            // Should show prices
            Assert.Contains("€", markup); // Price symbol should be present
        });
    }
}