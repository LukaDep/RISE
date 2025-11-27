using System.Text.Json;
using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using Rise.Services.Schedule;
using Rise.Shared.Common;
using Rise.Services.Tests.TestInfrastructure;

namespace Rise.Services.Tests.Schedule;

public class ScheduleServiceShould
{
    private static IDisposable UseWorkingDirectory(string path)
    {
        var prev = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(path);
        return new ResetDir(prev);
    }

    private sealed class ResetDir(string prev) : IDisposable
    {
        public void Dispose() => Directory.SetCurrentDirectory(prev);
    }

    [Fact]
    public async Task GetIndexAsyncShouldReturnSuccessWithValidData()
    {
        var tempRoot = Directory.CreateTempSubdirectory();
        try
        {
            var cwd = Path.Combine(tempRoot.FullName, "cwd");
            Directory.CreateDirectory(cwd);
            var mockDir = Path.Combine(tempRoot.FullName, "Rise.Services", "Schedule", "MockData");
            Directory.CreateDirectory(mockDir);

            var mockData = new
            {
                columnheaders = new[] { "Olod", "Werkvorm", "Onderwerp", "Info werkvorm", "Leer- of toetsomgeving", "Lokaal", "Lesgever" },
                info = new { schedulelimit = 1000, schedulecount = 1 },
                schedules = new[]
                {
                    new
                    {
                        id = "sched-001",
                        startdate = "01-09-2025",
                        starttime = "08:30",
                        enddate = "01-09-2025",
                        endtime = "10:30",
                        columns = new[] { "Web Ontwikkeling 2", "Hoorcollege", "", "", "Digitaal (laptop/PC)", "GSCHB.2.009", "Bert Van Vreckem" }
                    }
                }
            };
            var json = JsonSerializer.Serialize(mockData);
            var filePath = Path.Combine(mockDir, "ScheduleMockdata.json");
            await File.WriteAllTextAsync(filePath, json);

            using var _ = UseWorkingDirectory(cwd); // so ..\Rise.Services\Schedule\MockData resolves into tempRoot
            using var fixture = new SqliteTestFixture();
            using var dbContext = fixture.CreateContext();
            var service = new MockScheduleService(dbContext);
            var request = new QueryRequest.SkipTake { Skip = 0, Take = 10 };
            var result = await service.GetIndexAsync(request, CancellationToken.None);
            if (result.IsSuccess)
            {
                result.Value.ShouldNotBeNull();
                result.Value.Schedules.ShouldNotBeNull();
                result.Value.Schedules.Count.ShouldBeGreaterThan(0);
            }
            else
            {
                result.Status.ShouldBe(ResultStatus.NotFound);
            }
        }
        finally
        {
            tempRoot.Delete(recursive: true);
        }
    }

    [Fact]
    public void ConvertToDtoCorrectly()
    {
        var mockData = new
        {
            columnheaders = new[] { "Olod", "Werkvorm", "Onderwerp", "Info werkvorm", "Leer- of toetsomgeving", "Lokaal", "Lesgever" },
            info = new { schedulelimit = 1000, schedulecount = 2 },
            schedules = new[]
            {
                new
                {
                    id = "test-001",
                    startdate = "01-09-2025",
                    starttime = "08:30",
                    enddate = "01-09-2025",
                    endtime = "10:30",
                    columns = new[] { "Web Ontwikkeling 2", "Hoorcollege", "", "", "Digitaal (laptop/PC)", "GSCHB.2.009", "Bert Van Vreckem" }
                },
                new
                {
                    id = "test-002",
                    startdate = "02-09-2025",
                    starttime = "11:00",
                    enddate = "02-09-2025",
                    endtime = "13:00",
                    columns = new[] { "Databanken II", "Activerend hoorcollege", "", "", "Digitaal (laptop/PC)", "GSCHB.3.012", "Thomas Parmentier" }
                }
            }
        };

        var json = JsonSerializer.Serialize(mockData);

        var result = MockScheduleService.ConvertToDto(json);

        result.ShouldNotBeNull();
        result.Schedules.Count.ShouldBe(2);

        var firstSchedule = result.Schedules[0];
        firstSchedule.Id.ShouldBe("test-001");
        firstSchedule.Course.ShouldBe("Web Ontwikkeling 2");
        firstSchedule.WorkForm.ShouldBe("Hoorcollege");
        firstSchedule.Environment.ShouldBe("Digitaal (laptop/PC)");
        firstSchedule.Room.ShouldBe("GSCHB.2.009");
        firstSchedule.Teacher.ShouldBe("Bert Van Vreckem");
        firstSchedule.StartDateTime.ShouldBe(new DateTime(2025, 9, 1, 8, 30, 0));
        firstSchedule.EndDateTime.ShouldBe(new DateTime(2025, 9, 1, 10, 30, 0));

        var secondSchedule = result.Schedules[1];
        secondSchedule.Id.ShouldBe("test-002");
        secondSchedule.Course.ShouldBe("Databanken II");
        secondSchedule.WorkForm.ShouldBe("Activerend hoorcollege");
        secondSchedule.Teacher.ShouldBe("Thomas Parmentier");
        secondSchedule.StartDateTime.ShouldBe(new DateTime(2025, 9, 2, 11, 0, 0));
        secondSchedule.EndDateTime.ShouldBe(new DateTime(2025, 9, 2, 13, 0, 0));
    }

    [Fact]
    public void EmptyColumnsShouldReturnEmptySchedule()
    {
        var mockData = new
        {
            columnheaders = new[] { "Olod", "Werkvorm" },
            info = new { schedulelimit = 1000, schedulecount = 1 },
            schedules = new[]
            {
                new
                {
                    id = "test-003",
                    startdate = "03-09-2025",
                    starttime = "14:00",
                    enddate = "03-09-2025",
                    endtime = "16:00",
                    columns = Array.Empty<string>() // Empty columns array
                }
            }
        };

        var json = JsonSerializer.Serialize(mockData);

        var result = MockScheduleService.ConvertToDto(json);

        result.ShouldNotBeNull();
        result.Schedules.Count.ShouldBe(1);

        var schedule = result.Schedules[0];
        schedule.Id.ShouldBe("test-003");
        schedule.Course.ShouldBe(string.Empty);
        schedule.WorkForm.ShouldBe(string.Empty);
        schedule.Environment.ShouldBe(string.Empty);
        schedule.Room.ShouldBe(string.Empty);
        schedule.Teacher.ShouldBe(string.Empty);
    }

    [Fact]
    public void HandleMultipleSchedulesCorrectly()
    {
        var mockData = new
        {
            columnheaders = new[] { "Olod", "Werkvorm", "Onderwerp", "Info werkvorm", "Leer- of toetsomgeving", "Lokaal", "Lesgever" },
            info = new { schedulelimit = 1000, schedulecount = 3 },
            schedules = new[]
            {
                new
                {
                    id = "test-005",
                    startdate = "05-09-2025",
                    starttime = "08:00",
                    enddate = "05-09-2025",
                    endtime = "10:00",
                    columns = new[] { "Course 1", "Type 1", "", "", "Env 1", "Room 1", "Teacher 1" }
                },
                new
                {
                    id = "test-006",
                    startdate = "05-09-2025",
                    starttime = "10:30",
                    enddate = "05-09-2025",
                    endtime = "12:30",
                    columns = new[] { "Course 2", "Type 2", "", "", "Env 2", "Room 2", "Teacher 2" }
                },
                new
                {
                    id = "test-007",
                    startdate = "05-09-2025",
                    starttime = "13:00",
                    enddate = "05-09-2025",
                    endtime = "15:00",
                    columns = new[] { "Course 3", "Type 3", "", "", "Env 3", "Room 3", "Teacher 3" }
                }
            }
        };

        var json = JsonSerializer.Serialize(mockData);

        var result = MockScheduleService.ConvertToDto(json);

        result.ShouldNotBeNull();
        result.Schedules.Count.ShouldBe(3);
        result.Schedules[0].Id.ShouldBe("test-005");
        result.Schedules[1].Id.ShouldBe("test-006");
        result.Schedules[2].Id.ShouldBe("test-007");
    }

    [Fact]
    public void ParseDateTimeCorrectly()
    {
        var mockData = new
        {
            columnheaders = new[] { "Olod" },
            info = new { schedulelimit = 1000, schedulecount = 1 },
            schedules = new[]
            {
                new
                {
                    id = "test-008",
                    startdate = "15-12-2025",
                    starttime = "23:59",
                    enddate = "16-12-2025",
                    endtime = "01:30",
                    columns = new[] { "Test Course" }
                }
            }
        };

        var json = JsonSerializer.Serialize(mockData);

        var result = MockScheduleService.ConvertToDto(json);

        var schedule = result.Schedules[0];
        schedule.StartDateTime.ShouldBe(new DateTime(2025, 12, 15, 23, 59, 0));
        schedule.EndDateTime.ShouldBe(new DateTime(2025, 12, 16, 1, 30, 0));
    }
}
