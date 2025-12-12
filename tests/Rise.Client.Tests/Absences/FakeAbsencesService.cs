using Ardalis.Result;
using Rise.Shared.Absences;
using Rise.Shared.Common;

namespace Rise.Client.Absences;

public class FakeAbsencesService : IAbsencesService
{
    private readonly List<AbsenceDto.Index> _absences = new()
    {
        new AbsenceDto.Index
        {
            Id = new Guid("11111111-1111-1111-1111-111111111111"),
            Name = "Medical Leave",
            StartDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2024, 1, 20, 0, 0, 0, DateTimeKind.Utc),
            Reason = "Surgery recovery"
        },
        new AbsenceDto.Index
        {
            Id = new Guid("22222222-2222-2222-2222-222222222222"),
            Name = "Personal Leave",
            StartDate = new DateTime(2024, 2, 10, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2024, 2, 12, 0, 0, 0, DateTimeKind.Utc),
            Reason = "Family event"
        },
        new AbsenceDto.Index
        {
            Id = new Guid("33333333-3333-3333-3333-333333333333"),
            Name = "Sick Leave",
            StartDate = new DateTime(2024, 3, 5, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2024, 3, 8, 0, 0, 0, DateTimeKind.Utc),
            Reason = "Flu"
        }
    };

    public Task<Result<AbsencesResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var query = _absences.AsEnumerable();

        // Apply ordering
        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            query = request.OrderBy.ToLower() switch
            {
                "name" => request.OrderDescending 
                    ? query.OrderByDescending(a => a.Name) 
                    : query.OrderBy(a => a.Name),
                "startdate" => request.OrderDescending 
                    ? query.OrderByDescending(a => a.StartDate) 
                    : query.OrderBy(a => a.StartDate),
                "enddate" => request.OrderDescending 
                    ? query.OrderByDescending(a => a.EndDate) 
                    : query.OrderBy(a => a.EndDate),
                _ => query.OrderBy(a => a.StartDate)
            };
        }
        else
        {
            // Default ordering by StartDate ascending
            query = query.OrderBy(a => a.StartDate);
        }

        // Apply pagination
        var result = query.Skip(request.Skip).Take(request.Take).ToList();

        var response = new AbsencesResponse.Index
        {
            Absences = result
        };

        return Task.FromResult(Result.Success(response));
    }
}
