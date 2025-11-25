using Microsoft.Extensions.Localization;
using Xunit.Abstractions;

namespace Rise.Client.Pages;

/// <summary>
/// These tests are written entirely in C#.
/// Learn more at https://bunit.dev/docs/getting-started/writing-tests.html#creating-basic-tests-in-cs-files
/// </summary>
public class IndexShould : TestContext
{
    public IndexShould(ITestOutputHelper outputHelper)
    {
        Services.AddXunitLogger(outputHelper);
        Services.AddLocalization();
    }

    [Fact]
    public void RendersLocalizedComingSoonHeader()
    {
        // Arrange & Act
        var cut = RenderComponent<Index>();

        // Assert: header uses localized value for Common.ComingSoon
        var localizer = Services.GetRequiredService<IStringLocalizer<SharedResources>>();
        Assert.Contains($"<h1>{localizer["Common.ComingSoon"]}</h1>", cut.Markup);
    }
}