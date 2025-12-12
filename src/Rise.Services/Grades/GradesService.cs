using Rise.Persistence;
using Rise.Services.Identity;

namespace Rise.Services.Grades;

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Rise.Shared.Common;
using Rise.Shared.Grades;

/// <summary>
/// Service for managing student grades and results.
/// </summary>
public class GradesService(ApplicationDbContext dbContext, ISessionContextProvider sessionContextProvider) : IGradesService
{
    /// <summary>
    /// Retrieves the current user's ID from the session context.
    /// Returns an empty string if the user is not authenticated.
    /// </summary>
    private string GetCurrentUserId()
    {
        var userId = sessionContextProvider.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return userId ?? string.Empty;
    }

    /// <summary>
    /// Retrieves a filtered and paginated list of grades for the current user.
    /// Supports searching by course name and name, and sorting. Defaults to sorting by date (newest first).
    /// Returns an empty list if the user is not logged in.
    /// </summary>
    /// <param name="request">QueryRequest.SkipTake with SearchTerm, OrderBy, Skip and Take</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result with GradesResponse.Index containing the list of grades</returns>
    public async Task<Result<GradesResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Result.Success(new GradesResponse.Index
            {
                Grades = []
            });
        }
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
            .Where(g => g.UserId == userId)
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

    /// <summary>
    /// Retrieves a specific grade record by ID for the current user.
    /// Only the owner can view their own grades.
    /// </summary>
    /// <param name="id">The Guid of the grade record to retrieve</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result with GradesResponse.Get containing the grade, Unauthorized if not logged in, or NotFound if not found</returns>
    public async Task<Result<GradesResponse.Get>> GetGradeByIdAsync(Guid id, CancellationToken ctx)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Result<GradesResponse.Get>.Unauthorized("User is not authenticated");
        }
        var query = dbContext.Grades.AsQueryable();
        var grade = await query.AsNoTracking()
            .Where(g => g.Id.Equals(id))
            .Where(g => g.UserId == userId)
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