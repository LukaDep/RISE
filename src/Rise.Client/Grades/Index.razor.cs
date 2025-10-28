using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.Grades;

namespace Rise.Client.Grades;

public partial class Index
{
    private IEnumerable<GradesDto.Course>? Courses;
    [Inject] public required IGradesService GradesClientService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 50,
            OrderBy = "CourseId",
        };

        var result = await GradesClientService.GetCoursesAsync(request);
        Courses = result.Value.Courses;
    }
}