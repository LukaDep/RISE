using Microsoft.AspNetCore.Components;
using Rise.Shared.Grades;
using Rise.Client.Components;

namespace Rise.Client.Grades.Components;

public partial class GradeItem : ComponentBase
{
    [Parameter] public GradesDto.Grade Grade { get; set; } = default!;
    private Boolean seeDetails = false;
    private void SeeGradeDetails()
    {
        seeDetails = !seeDetails;
    }

    private double ScorePercentage
    {
        get
        {
            if (Grade?.Score.HasValue == true && Grade?.MaxPoints.HasValue == true && Grade.MaxPoints > 0)
            {
                return Math.Round((Grade.Score.Value / Grade.MaxPoints.Value) * 100, 1);
            }
            return 0;
        }
    }

    private bool IsPassing => ScorePercentage >= 50;

    private string ScoreBarClass
    {
        get
        {
            if (!(Grade?.Score.HasValue ?? false) || !(Grade?.MaxPoints.HasValue ?? false) || Grade.MaxPoints <= 0)
                return "bg-hogent-black-30";

            return IsPassing
                ? "bg-gradient-to-r from-green-500 to-green-400"
                : "bg-gradient-to-r from-red-500 to-red-400";
        }
    }

    private string ScoreBadgeClass
    {
        get
        {
            if (!(Grade?.Score.HasValue ?? false) || !(Grade?.MaxPoints.HasValue ?? false) || Grade.MaxPoints <= 0)
                return "bg-hogent-black-10 text-hogent-black";

            return IsPassing
                ? "bg-green-500 text-white"
                : "bg-red-500 text-white";
        }
    }

    private string ScoreTextClass
    {
        get
        {
            if (!(Grade?.Score.HasValue ?? false) || !(Grade?.MaxPoints.HasValue ?? false) || Grade.MaxPoints <= 0)
                return "text-hogent-black";

            return IsPassing ? "text-green-600" : "text-red-600";
        }
    }
}
