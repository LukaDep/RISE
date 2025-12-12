using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Rise.Client.Components;
using Xunit;

namespace Rise.Client.Tests.Components;

public class FilterLabelsShould : TestContext
{
    public FilterLabelsShould()
    {
        // Add a FakeNavigationManager since FilterLabel uses NavigationManager
        Services.AddSingleton<NavigationManager>(new FakeNavigationManager(this));
        Services.AddLocalization();
    }

    #region Rendering

    [Fact]
    public void RenderEmptySpan_WhenNoFilters()
    {
        // Act
        var cut = RenderComponent<FilterLabels>(parameters => parameters
            .Add(p => p.Filters, new List<string>()));

        // Assert
        var span = cut.Find("span");
        Assert.Empty(span.QuerySelectorAll("p"));
    }

    [Fact]
    public void RenderSingleFilter_WhenOneFilterProvided()
    {
        // Arrange
        var filters = new List<string> { "Filter1" };

        // Act
        var cut = RenderComponent<FilterLabels>(parameters => parameters
            .Add(p => p.Filters, filters));

        // Assert
        Assert.Contains("Filter1", cut.Markup);
    }

    [Fact]
    public void RenderMultipleFilters_WhenMultipleFiltersProvided()
    {
        // Arrange
        var filters = new List<string> { "Filter1", "Filter2", "Filter3" };

        // Act
        var cut = RenderComponent<FilterLabels>(parameters => parameters
            .Add(p => p.Filters, filters));

        // Assert
        Assert.Contains("Filter1", cut.Markup);
        Assert.Contains("Filter2", cut.Markup);
        Assert.Contains("Filter3", cut.Markup);
    }

    [Fact]
    public void RenderFilterLabels_ForEachFilter()
    {
        // Arrange
        var filters = new List<string> { "Category A", "Category B" };

        // Act
        var cut = RenderComponent<FilterLabels>(parameters => parameters
            .Add(p => p.Filters, filters));

        // Assert
        var labels = cut.FindAll("p");
        Assert.Equal(2, labels.Count);
    }

    #endregion

    #region Styling

    [Fact]
    public void HaveCorrectContainerClasses()
    {
        // Arrange
        var filters = new List<string> { "Filter1" };

        // Act
        var cut = RenderComponent<FilterLabels>(parameters => parameters
            .Add(p => p.Filters, filters));

        // Assert
        var span = cut.Find("span");
        Assert.Contains("ml-4", span.ClassList);
        Assert.Contains("flex", span.ClassList);
        Assert.Contains("flex-row", span.ClassList);
        Assert.Contains("gap-2", span.ClassList);
    }

    #endregion

    #region FakeNavigationManager

    private class FakeNavigationManager : NavigationManager
    {
        private readonly TestContext _ctx;

        public FakeNavigationManager(TestContext ctx)
        {
            _ctx = ctx;
            Initialize("http://localhost/", "http://localhost/");
        }

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            Uri = new Uri(new Uri(BaseUri), uri).ToString();
            NotifyLocationChanged(false);
        }
    }

    #endregion
}
