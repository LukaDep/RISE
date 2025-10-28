namespace Rise.Client.Grades.Components;

using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class CourseItem : ComponentBase
{
    [Parameter] public GradesDto.Course Course { get; set; } = default!;
    [Parameter] public EventCallback OnClick { get; set; }

    protected int TotalGrades { get; set; }
    protected int SubmittedCount { get; set; }
    protected int SubmittedPercent { get; set; } = 0; // NEW: percent for circle
    protected string AveragePercentageText { get; set; } = "-";
    protected string SubmittedText { get; set; } = "-";
    protected string FinalScoreText { get; set; } = "Pending";
    protected string TotalGradesText => $"{SubmittedCount}/{TotalGrades} submitted";

    // circle geometry (r = 18)
    private const double Radius = 18.0;
    protected double Circumference => 2.0 * Math.PI * Radius;
    protected string CircumferenceString => Circumference.ToString("F3", System.Globalization.CultureInfo.InvariantCulture);
    protected double StrokeDashOffset => Circumference * (1.0 - SubmittedPercent / 100.0);
    protected string StrokeDashOffsetString => StrokeDashOffset.ToString("F3", System.Globalization.CultureInfo.InvariantCulture);

    protected override void OnParametersSet()
    {
        ComputeMetrics();
    }

    private void ComputeMetrics()
    {
        var grades = Course.Grades ?? new List<GradesDto.Grade>();

        TotalGrades = grades.Count;
        SubmittedCount = grades.Count(g => g?.SubmissionDate != null);

        // update submitted percent used by circle
        SubmittedPercent = TotalGrades == 0 ? 0 : (int)Math.Round(SubmittedCount * 100.0 / TotalGrades);

        // average percentage
        var scored = grades.Where(g => g.MaxPoints != null && g?.Score != null && g.MaxPoints > 0).ToList();
        if (scored.Count != 0)
        {
            var avgPercent = scored.Average(g => (double)g.Score!.Value / g.MaxPoints * 100.0);
            AveragePercentageText = $"{avgPercent:F1}%";
        }
        else
        {
            AveragePercentageText = "-";
        }

        // submitted percentage text
        SubmittedText = $"{SubmittedPercent}% ({SubmittedCount}/{TotalGrades})";

        // final score of the whole course, at the end of semester
        if (Course.FinalScore.HasValue)
        {
            FinalScoreText = $"{Course.FinalScore.Value:F2}/20";
        }
        else
        {
            FinalScoreText = "-";
        }
    }

    protected Task HandleClick()
    {
        return OnClick.HasDelegate ? OnClick.InvokeAsync(null) : Task.CompletedTask;
    }
}
