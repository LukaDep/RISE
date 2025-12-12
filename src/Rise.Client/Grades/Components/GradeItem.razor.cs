using Microsoft.AspNetCore.Components;
using Rise.Shared.Grades;
using Rise.Client.Components;

namespace Rise.Client.Grades.Components;

/// <summary>
/// Component that displays a single grade item with progress bar.
/// Shows course name, score, and expandable details.
/// </summary>
public partial class GradeItem : ComponentBase
{
    /// <summary>The grade data to display.</summary>
    [Parameter] public GradesDto.Grade Grade { get; set; } = default!;
    
    private Boolean seeDetails = false;
    
    /// <summary>
    /// Toggles the grade details visibility.
    /// </summary>
    private void SeeGradeDetails()
    {
        seeDetails = !seeDetails;
    }

    /// <summary>
    /// Calculates the score as a percentage of maximum points.
    /// Returns 0 if score or max points are not available or max points is zero.
    /// </summary>
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

    /// <summary>
    /// Determines if the grade is passing (>=50% of max points).
    /// </summary>
    private bool IsPassing => ScorePercentage >= 50;

    /// <summary>
    /// Gets the CSS classes for the score progress bar.
    /// Returns gray for no score, green gradient for passing (>=50%), 
    /// or red gradient for failing (<50%).
    /// </summary>
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

    /// <summary>
    /// Gets the CSS classes for the score badge.
    /// Returns gray styling for no score, green with white text for passing,
    /// or red with white text for failing.
    /// </summary>
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

    /// <summary>
    /// Gets the CSS class for the score text color.
    /// Returns black for no score, green for passing, or red for failing.
    /// </summary>
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
