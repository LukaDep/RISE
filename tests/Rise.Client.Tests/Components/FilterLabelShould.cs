using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Rise.Client.Components;
using Xunit;

namespace Rise.Client.Tests.Components;

public class FilterLabelShould : TestContext
{
    private readonly FakeNavigationManager _navigationManager;

    public FilterLabelShould()
    {
        _navigationManager = new FakeNavigationManager(this);
        Services.AddSingleton<NavigationManager>(_navigationManager);
        Services.AddLocalization();
    }

    #region Rendering

    [Fact]
    public void RenderFilterText()
    {
        // Act
        var cut = RenderComponent<FilterLabel>(parameters => parameters
            .Add(p => p.Filter, "Test Filter"));

        // Assert
        Assert.Contains("Test Filter", cut.Markup);
    }

    [Fact]
    public void RenderWithInactiveStyles_WhenNotInQuery()
    {
        // Arrange
        _navigationManager.NavigateTo("http://localhost/page");

        // Act
        var cut = RenderComponent<FilterLabel>(parameters => parameters
            .Add(p => p.Filter, "MyFilter"));

        // Assert
        var paragraph = cut.Find("p");
        Assert.Contains("border-hogent-black-15", paragraph.ClassList);
        Assert.Contains("bg-hogent-black-20", paragraph.ClassList);
    }

    [Fact]
    public void RenderWithActiveStyles_WhenInQuery()
    {
        // Arrange
        _navigationManager.NavigateTo("http://localhost/page?filter=MyFilter");

        // Act
        var cut = RenderComponent<FilterLabel>(parameters => parameters
            .Add(p => p.Filter, "MyFilter"));

        // Assert
        var paragraph = cut.Find("p");
        Assert.Contains("border-hogent-black-50", paragraph.ClassList);
        Assert.Contains("bg-hogent-education-50", paragraph.ClassList);
    }

    #endregion

    #region Click Behavior

    [Fact]
    public void AddFilterToQuery_WhenClicked_AndNotActive()
    {
        // Arrange
        _navigationManager.NavigateTo("http://localhost/page");

        var cut = RenderComponent<FilterLabel>(parameters => parameters
            .Add(p => p.Filter, "NewFilter"));

        // Act
        cut.Find("p").Click();

        // Assert
        Assert.Contains("filter=NewFilter", _navigationManager.Uri);
    }

    [Fact]
    public void ReplaceExistingFilter_WhenClicked()
    {
        // Arrange - FilterLabel replaces filter rather than appending
        _navigationManager.NavigateTo("http://localhost/page?filter=ExistingFilter");

        var cut = RenderComponent<FilterLabel>(parameters => parameters
            .Add(p => p.Filter, "NewFilter"));

        // Act
        cut.Find("p").Click();

        // Assert - should replace filter with NewFilter
        Assert.Contains("filter=NewFilter", _navigationManager.Uri);
    }

    [Fact]
    public void RemoveFilterFromQuery_WhenClicked_AndActive()
    {
        // Arrange
        _navigationManager.NavigateTo("http://localhost/page?filter=ActiveFilter");

        var cut = RenderComponent<FilterLabel>(parameters => parameters
            .Add(p => p.Filter, "ActiveFilter"));

        // Act
        cut.Find("p").Click();

        // Assert - filter should be removed (empty value)
        Assert.DoesNotContain("filter=ActiveFilter", _navigationManager.Uri);
    }

    [Fact]
    public void PreserveOtherQueryParams_WhenChangingFilter()
    {
        // Arrange
        _navigationManager.NavigateTo("http://localhost/page?other=value&filter=OldFilter");

        var cut = RenderComponent<FilterLabel>(parameters => parameters
            .Add(p => p.Filter, "NewFilter"));

        // Act
        cut.Find("p").Click();

        // Assert - should keep other params
        Assert.Contains("other=value", _navigationManager.Uri);
    }

    #endregion

    #region Location Changed

    [Fact]
    public void UpdateIsActive_WhenLocationChanges()
    {
        // Arrange
        _navigationManager.NavigateTo("http://localhost/page");

        var cut = RenderComponent<FilterLabel>(parameters => parameters
            .Add(p => p.Filter, "MyFilter"));

        // Initially not active
        var paragraph = cut.Find("p");
        Assert.Contains("border-hogent-black-15", paragraph.ClassList);

        // Act - navigate to URL with filter
        _navigationManager.NavigateTo("http://localhost/page?filter=MyFilter");

        // Assert - now active
        paragraph = cut.Find("p");
        Assert.Contains("border-hogent-black-50", paragraph.ClassList);
    }

    [Fact]
    public void UpdateIsActive_WhenFilterRemovedFromUrl()
    {
        // Arrange
        _navigationManager.NavigateTo("http://localhost/page?filter=MyFilter");

        var cut = RenderComponent<FilterLabel>(parameters => parameters
            .Add(p => p.Filter, "MyFilter"));

        // Initially active
        var paragraph = cut.Find("p");
        Assert.Contains("border-hogent-black-50", paragraph.ClassList);

        // Act - navigate to URL without filter
        _navigationManager.NavigateTo("http://localhost/page");

        // Assert - now inactive
        paragraph = cut.Find("p");
        Assert.Contains("border-hogent-black-15", paragraph.ClassList);
    }

    #endregion

    #region Disposal

    [Fact]
    public void DisposeCorrectly()
    {
        // Arrange
        var cut = RenderComponent<FilterLabel>(parameters => parameters
            .Add(p => p.Filter, "Test"));

        // Act - dispose should not throw
        cut.Dispose();

        // Assert - no exception thrown means success
        Assert.True(true);
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

        public new void NavigateTo(string uri, bool forceLoad = false, bool replace = false)
        {
            Uri = uri;
            NotifyLocationChanged(false);
        }

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            Uri = new Uri(new Uri(BaseUri), uri).ToString();
            NotifyLocationChanged(false);
        }
    }

    #endregion
}
