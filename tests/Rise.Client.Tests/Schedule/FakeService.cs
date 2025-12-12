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
    public Task<Result<ScheduleDto.Data>> GetIndexAsync(QueryRequest.DateRange req, CancellationToken ctx = default)
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
    // Use today's date to ensure tests pass regardless of when they run
    private static readonly DateTime Today = DateTime.Now.Date;
    
    private readonly List<ScheduleDto.Schedule> _items = new()
    {
        new ScheduleDto.Schedule { Id = "s1", Course = "Web Ontwikkeling 2", WorkForm = "Hoorcollege", Environment = "Digitaal", Room = "GSCHB.2.009", Teacher = "Bert Van Vreckem", StartDateTime = Today.AddHours(8).AddMinutes(30), EndDateTime = Today.AddHours(10).AddMinutes(30), IsAbsent = false },
        new ScheduleDto.Schedule { Id = "s2", Course = "Databanken II", WorkForm = "Activerend hoorcollege", Environment = "Digitaal", Room = "GSCHB.3.012", Teacher = "Thomas Parmentier", StartDateTime = Today.AddDays(1).AddHours(11), EndDateTime = Today.AddDays(1).AddHours(13), IsAbsent = true },
        new ScheduleDto.Schedule { Id = "s3", Course = "Software Engineering", WorkForm = "Werkcollege", Environment = "Fysiek", Room = "GSCHB.1.005", Teacher = "Jan Janssens", StartDateTime = Today.AddDays(2).AddHours(14), EndDateTime = Today.AddDays(2).AddHours(16), IsAbsent = true }
    };

    public Task<Result<ScheduleDto.Data>> GetIndexAsync(QueryRequest.DateRange req, CancellationToken ctx = default)
    {
        var query = _items.AsEnumerable();

        // Apply date range filtering
        query = ScheduleDto.ApplyDateRangeFilter(query, req);

        // Apply search filter if SearchTerm is provided
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

    public void CreateMultipleSchedulesForSameDay(DateTime date, int count)
    {
        for (int i = 0; i < count; i++)
        {
            _items.Add(new ScheduleDto.Schedule
            {
                Id = $"temp_{i}",
                Course = $"Test Course {i}",
                WorkForm = i % 2 == 0 ? "Hoorcollege" : "Werkcollege",
                Environment = "Digitaal",
                Room = $"ROOM.{i}",
                Teacher = $"Teacher {i}",
                StartDateTime = date.AddHours(8 + i),
                EndDateTime = date.AddHours(10 + i),
                IsAbsent = false
            });
        }
    }
}
