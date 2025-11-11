using Rise.Persistence;

namespace Rise.Services.Grades;

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Rise.Shared.Common;
using Rise.Shared.Grades;

/// <summary>
/// Service for grades.
/// </summary>

public class GradesService(ApplicationDbContext dbContext) : IGradesService
{
    public async Task<Result<GradesResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var query = dbContext.Grades.AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(g => (g.CourseName ?? string.Empty).Contains(request.SearchTerm)
                                     || (g.Name ?? string.Empty).Contains(request.SearchTerm));
        }
        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            query = request.OrderDescending
                ? query.OrderByDescending(e => EF.Property<object>(e, request.OrderBy))
                : query.OrderBy(e => EF.Property<object>(e, request.OrderBy));
        }
        else
        {
            query = query.OrderByDescending(g => g.Date);
        }

        var grades = await query.AsNoTracking()
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(g => new GradesDto.Grade
            {
                Id = g.Id,
                Name = g.Name,
                ActivityType = g.ActivityType,
                MaxPoints = g.MaxPoints,
                Score = g.Score,
                Feedback = g.Feedback,
                SubmissionDate = g.SubmissionDate,
                Date = g.Date,
                CourseId = g.CourseId,
                CourseName = g.CourseName,
                Year = g.Year,
                Semester = g.Semester
            }).ToListAsync(ctx);
        return Result.Success(new GradesResponse.Index { Grades = grades });
    }

    public async Task<Result<GradesResponse.Get>> GetGradeByIdAsync(Guid id, CancellationToken ctx)
    {
        var query = dbContext.Grades.AsQueryable();
        var grade = await query.AsNoTracking()
            .Where(g => g.Id.Equals(id))
            .Select(g => new GradesDto.Grade
            {
                Id = g.Id,
                Name = g.Name,
                ActivityType = g.ActivityType,
                MaxPoints = g.MaxPoints,
                Score = g.Score,
                Feedback = g.Feedback,
                SubmissionDate = g.SubmissionDate,
                Date = g.Date,
                CourseId = g.CourseId,
                CourseName = g.CourseName,
                Year = g.Year,
                Semester = g.Semester
            }).FirstOrDefaultAsync(ctx);
        return grade == null ? Result<GradesResponse.Get>.NotFound($"Grade with id {id} not found") : Result.Success(new GradesResponse.Get { Grade = grade });
    }
}