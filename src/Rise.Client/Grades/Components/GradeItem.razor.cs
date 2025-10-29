using Microsoft.AspNetCore.Components;
using Rise.Shared.Grades;

namespace Rise.Client.Grades.Components;

public partial class GradeItem : ComponentBase
{
    [Parameter] public GradesDto.Grade Grade { get; set; } = default!;
    private Boolean seeDetails = false;
    private void SeeGradeDetails()
    {
        seeDetails = !seeDetails;
    }

    private string CardBackgroundClass
    {
        get
        {
            if (Grade?.Score.HasValue == true && Grade?.MaxPoints.HasValue == true && Grade.MaxPoints > 0)
            {
                var score = Grade.Score.GetValueOrDefault();
                var max = Grade.MaxPoints.GetValueOrDefault();
                if (score < 0.5 * max)
                {
                    return "bg-hogent-it-15 border-hogent-it-50";
                }
            }

            return "bg-hogent-white border-hogent-black-15";
        }
    }
}
