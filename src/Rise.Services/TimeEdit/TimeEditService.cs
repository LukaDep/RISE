using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Services.Identity;
using Rise.Shared.Identity;
using Rise.Shared.Projects;

namespace Rise.Services.TimeEdits;

/// <summary>
/// Service for managing and retrieving TimeEdit data (read-only schedule view).
/// </summary>
public class TimeEditService(ApplicationDbContext dbContext) : ITimeEditService
{
    /// <summary>
    /// Retrieve schedule data from TimeEdit.
    /// </summary>
    public async Task<Result<TimeEditResponse.Schedule>> GetTimeEditData(TimeEditRequest.Schedule request, CancellationToken ctx = default)
    {
        return {
            data:
        }
    }
}
