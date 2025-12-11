namespace Rise.Client.Grades;

using Rise.Shared.Grades;
using Rise.Client.Tests.Grades;

public class IndexOptionsShould : TestContext
{
    public IndexOptionsShould()
    {
        Services.AddLocalization();
        Services.AddScoped<IGradesService, FakeGradesService>();
    }

    [Fact]
    public void YearOptionsContainSixEntries_AndFirstIsCurrentStartYearRange()
    {
        var cut = RenderComponent<Index>();

        var options = cut.Instance.YearOptions;

        Assert.Equal(6, options.Count);

        var now = DateTime.Now;
        var currentStartYear = now.Month >= 9 ? now.Year : now.Year - 1;
        var expected = $"{currentStartYear}-{currentStartYear + 1}";

        Assert.Equal(expected, options[0]);
    }
}
