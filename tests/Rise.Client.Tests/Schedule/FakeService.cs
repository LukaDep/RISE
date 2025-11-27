using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Rise.Shared.Common;
using Rise.Shared.Schedule;

namespace Rise.Client.Schedule;

public class NullScheduleService : IScheduleService
{
    public Task<Result<ScheduleDto.Data>> GetIndexAsync(QueryRequest.SkipTake req, CancellationToken ctx = default)
    {
        var wrapper = new ScheduleDto.Data
        {
            Schedules = null!
        };

        return Task.FromResult(Result.Success(wrapper));
    }
}

public class FakeScheduleService : IScheduleService
{
    private readonly List<ScheduleDto.Schedule> _items = new()
    {
        new ScheduleDto.Schedule { Id = "s1", Course = "Web Ontwikkeling 2", WorkForm = "Hoorcollege", Environment = "Digitaal", Room = "GSCHB.2.009", Teacher = "Bert Van Vreckem", StartDateTime = DateTime.Today.AddHours(8).AddMinutes(30), EndDateTime = DateTime.Today.AddHours(10).AddMinutes(30) },
        new ScheduleDto.Schedule { Id = "s2", Course = "Databanken II", WorkForm = "Activerend hoorcollege", Environment = "Digitaal", Room = "GSCHB.3.012", Teacher = "Thomas Parmentier", StartDateTime = DateTime.Today.AddDays(1).AddHours(11), EndDateTime = DateTime.Today.AddDays(1).AddHours(13) }
    };

    public Task<Result<ScheduleDto.Data>> GetIndexAsync(QueryRequest.SkipTake req, CancellationToken ctx = default)
    {
        var query = _items.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(req?.SearchTerm))
        {
            var term = req.SearchTerm.Trim();
            query = query.Where(s =>
                (s.Course?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (s.Teacher?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (s.Room?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false));
        }
        query = !string.IsNullOrWhiteSpace(req?.OrderBy)
            ? req.OrderBy.ToLower() switch
            {
                "course" => req.OrderDescending
                    ? query.OrderByDescending(s => s.Course)
                    : query.OrderBy(s => s.Course),
                "teacher" => req.OrderDescending
                    ? query.OrderByDescending(s => s.Teacher)
                    : query.OrderBy(s => s.Teacher),
                "startdatetime" => req.OrderDescending
                    ? query.OrderByDescending(s => s.StartDateTime)
                    : query.OrderBy(s => s.StartDateTime),
                _ => req.OrderDescending
                    ? query.OrderByDescending(s => s.Id)
                    : query.OrderBy(s => s.Id)
            }
            : query.OrderBy(s => s.StartDateTime);
        if (req != null)
        {
            query = query.Skip(req.Skip).Take(req.Take);
        }

        var wrapper = new ScheduleDto.Data
        {
            Schedules = query.ToList()
        };

        return Task.FromResult(Result.Success(wrapper));
    }
}
