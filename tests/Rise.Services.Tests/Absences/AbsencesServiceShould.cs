using Ardalis.Result;
using Rise.Domain.Absences;
using Rise.Services.Absences;
using Rise.Shared.Common;
using Rise.Services.Tests.TestInfrastructure;

namespace Rise.Services.Tests.Absences;

public class AbsencesServiceShould : IDisposable
{
    private readonly SqliteTestFixture _fixture;

    public AbsencesServiceShould()
    {
        _fixture = new SqliteTestFixture();
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }

    [Fact]
    public async Task GetIndexAsync_Should_Return_Absences()
    {
        var (context, scope) = _fixture.CreateTransactionalContext();
        using (scope)
        {
            var absence1 = new Absence
            {
                Name = "Sick Leave",
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(2),
                Reason = "Flu"
            };
            var absence2 = new Absence
            {
                Name = "Personal Leave",
                StartDate = DateTime.UtcNow.Date.AddDays(5),
                EndDate = DateTime.UtcNow.Date.AddDays(7),
                Reason = "Family event"
            };

            context.Absences.AddRange(absence1, absence2);
            await context.SaveChangesAsync();

            var service = new AbsencesService(context);
            var request = new QueryRequest.SkipTake { Skip = 0, Take = 10 };

            var result = await service.GetIndexAsync(request);

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldNotBeNull();
            result.Value.Absences.ShouldNotBeNull();
            result.Value.Absences.Count().ShouldBe(2);
            result.Value.Absences.Select(a => a.Name).ShouldContain("Sick Leave");
            result.Value.Absences.Select(a => a.Name).ShouldContain("Personal Leave");
        }
    }

    [Fact]
    public async Task GetIndexAsync_Should_Apply_Pagination()
    {
        var (context, scope) = _fixture.CreateTransactionalContext();
        using (scope)
        {
            var absences = Enumerable.Range(1, 5).Select(i => new Absence
            {
                Name = $"Absence {i}",
                StartDate = DateTime.UtcNow.Date.AddDays(i),
                EndDate = DateTime.UtcNow.Date.AddDays(i + 1),
                Reason = $"Reason {i}"
            }).ToList();

            context.Absences.AddRange(absences);
            await context.SaveChangesAsync();

            var service = new AbsencesService(context);
            var request = new QueryRequest.SkipTake { Skip = 2, Take = 2 };

            var result = await service.GetIndexAsync(request);

            result.IsSuccess.ShouldBeTrue();
            result.Value.Absences.Count().ShouldBe(2);
        }
    }

    [Fact]
    public async Task GetIndexAsync_Should_Return_Empty_List_When_No_Absences()
    {
        var (context, scope) = _fixture.CreateTransactionalContext();
        using (scope)
        {
            var service = new AbsencesService(context);
            var request = new QueryRequest.SkipTake { Skip = 0, Take = 10 };

            var result = await service.GetIndexAsync(request);

            result.IsSuccess.ShouldBeTrue();
            result.Value.Absences.ShouldBeEmpty();
        }
    }

    [Fact]
    public async Task GetIndexAsync_Should_Order_By_StartDate_Ascending_By_Default()
    {
        var (context, scope) = _fixture.CreateTransactionalContext();
        using (scope)
        {
            var absence1 = new Absence
            {
                Name = "Future Absence",
                StartDate = DateTime.UtcNow.Date.AddDays(10),
                EndDate = DateTime.UtcNow.Date.AddDays(12),
                Reason = "Vacation"
            };
            var absence2 = new Absence
            {
                Name = "Current Absence",
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(2),
                Reason = "Medical"
            };
            var absence3 = new Absence
            {
                Name = "Past Absence",
                StartDate = DateTime.UtcNow.Date.AddDays(-5),
                EndDate = DateTime.UtcNow.Date.AddDays(-3),
                Reason = "Personal"
            };

            context.Absences.AddRange(absence1, absence2, absence3);
            await context.SaveChangesAsync();

            var service = new AbsencesService(context);
            var request = new QueryRequest.SkipTake { Skip = 0, Take = 10 };

            var result = await service.GetIndexAsync(request);

            result.IsSuccess.ShouldBeTrue();
            var absencesList = result.Value.Absences.ToList();
            absencesList[0].Name.ShouldBe("Past Absence");
            absencesList[1].Name.ShouldBe("Current Absence");
            absencesList[2].Name.ShouldBe("Future Absence");
        }
    }

    [Fact]
    public async Task GetIndexAsync_Should_Order_By_Name_Ascending()
    {
        var (context, scope) = _fixture.CreateTransactionalContext();
        using (scope)
        {
            var absence1 = new Absence
            {
                Name = "Zebra Leave",
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(1),
                Reason = "Test"
            };
            var absence2 = new Absence
            {
                Name = "Alpha Leave",
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(1),
                Reason = "Test"
            };
            var absence3 = new Absence
            {
                Name = "Beta Leave",
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(1),
                Reason = "Test"
            };

            context.Absences.AddRange(absence1, absence2, absence3);
            await context.SaveChangesAsync();

            var service = new AbsencesService(context);
            var request = new QueryRequest.SkipTake
            {
                Skip = 0,
                Take = 10,
                OrderBy = "Name",
                OrderDescending = false
            };

            var result = await service.GetIndexAsync(request);

            result.IsSuccess.ShouldBeTrue();
            var absencesList = result.Value.Absences.ToList();
            absencesList[0].Name.ShouldBe("Alpha Leave");
            absencesList[1].Name.ShouldBe("Beta Leave");
            absencesList[2].Name.ShouldBe("Zebra Leave");
        }
    }

    [Fact]
    public async Task GetIndexAsync_Should_Order_By_Name_Descending()
    {
        var (context, scope) = _fixture.CreateTransactionalContext();
        using (scope)
        {
            var absence1 = new Absence
            {
                Name = "Alpha Leave",
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(1),
                Reason = "Test"
            };
            var absence2 = new Absence
            {
                Name = "Beta Leave",
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(1),
                Reason = "Test"
            };
            var absence3 = new Absence
            {
                Name = "Zebra Leave",
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(1),
                Reason = "Test"
            };

            context.Absences.AddRange(absence1, absence2, absence3);
            await context.SaveChangesAsync();

            var service = new AbsencesService(context);
            var request = new QueryRequest.SkipTake
            {
                Skip = 0,
                Take = 10,
                OrderBy = "Name",
                OrderDescending = true
            };

            var result = await service.GetIndexAsync(request);

            result.IsSuccess.ShouldBeTrue();
            var absencesList = result.Value.Absences.ToList();
            absencesList[0].Name.ShouldBe("Zebra Leave");
            absencesList[1].Name.ShouldBe("Beta Leave");
            absencesList[2].Name.ShouldBe("Alpha Leave");
        }
    }

    [Fact]
    public async Task GetIndexAsync_Should_Order_By_EndDate_Descending()
    {
        var (context, scope) = _fixture.CreateTransactionalContext();
        using (scope)
        {
            var absence1 = new Absence
            {
                Name = "First",
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(1),
                Reason = "Test"
            };
            var absence2 = new Absence
            {
                Name = "Second",
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(5),
                Reason = "Test"
            };
            var absence3 = new Absence
            {
                Name = "Third",
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(3),
                Reason = "Test"
            };

            context.Absences.AddRange(absence1, absence2, absence3);
            await context.SaveChangesAsync();

            var service = new AbsencesService(context);
            var request = new QueryRequest.SkipTake
            {
                Skip = 0,
                Take = 10,
                OrderBy = "EndDate",
                OrderDescending = true
            };

            var result = await service.GetIndexAsync(request);

            result.IsSuccess.ShouldBeTrue();
            var absencesList = result.Value.Absences.ToList();
            absencesList[0].Name.ShouldBe("Second");
            absencesList[1].Name.ShouldBe("Third");
            absencesList[2].Name.ShouldBe("First");
        }
    }

    [Fact]
    public async Task GetIndexAsync_Should_Map_All_Properties_Correctly()
    {
        var (context, scope) = _fixture.CreateTransactionalContext();
        using (scope)
        {
            var startDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc);
            var endDate = new DateTime(2024, 1, 20, 0, 0, 0, DateTimeKind.Utc);

            var absence = new Absence
            {
                Name = "Medical Leave",
                StartDate = startDate,
                EndDate = endDate,
                Reason = "Surgery recovery"
            };

            context.Absences.Add(absence);
            await context.SaveChangesAsync();

            var service = new AbsencesService(context);
            var request = new QueryRequest.SkipTake { Skip = 0, Take = 10 };

            var result = await service.GetIndexAsync(request);

            result.IsSuccess.ShouldBeTrue();
            var absenceDto = result.Value.Absences.First();
            absenceDto.Id.ShouldBe(absence.Id);
            absenceDto.Name.ShouldBe("Medical Leave");
            absenceDto.StartDate.ShouldBe(startDate);
            absenceDto.EndDate.ShouldBe(endDate);
            absenceDto.Reason.ShouldBe("Surgery recovery");
        }
    }

    [Fact]
    public async Task GetIndexAsync_Should_Handle_CancellationToken()
    {
        var (context, scope) = _fixture.CreateTransactionalContext();
        using (scope)
        {
            var absence = new Absence
            {
                Name = "Test Absence",
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(1),
                Reason = "Test"
            };

            context.Absences.Add(absence);
            await context.SaveChangesAsync();

            var service = new AbsencesService(context);
            var request = new QueryRequest.SkipTake { Skip = 0, Take = 10 };
            var cts = new CancellationTokenSource();

            var result = await service.GetIndexAsync(request, cts.Token);

            result.IsSuccess.ShouldBeTrue();
        }
    }

    [Fact]
    public async Task GetIndexAsync_Should_Skip_Records_Correctly()
    {
        var (context, scope) = _fixture.CreateTransactionalContext();
        using (scope)
        {
            var absences = Enumerable.Range(1, 10).Select(i => new Absence
            {
                Name = $"Absence {i:D2}",
                StartDate = DateTime.UtcNow.Date.AddDays(i),
                EndDate = DateTime.UtcNow.Date.AddDays(i + 1),
                Reason = $"Reason {i}"
            }).ToList();

            context.Absences.AddRange(absences);
            await context.SaveChangesAsync();

            var service = new AbsencesService(context);
            var request = new QueryRequest.SkipTake { Skip = 5, Take = 10 };

            var result = await service.GetIndexAsync(request);

            result.IsSuccess.ShouldBeTrue();
            result.Value.Absences.Count().ShouldBe(5);
            result.Value.Absences.First().Name.ShouldBe("Absence 06");
        }
    }

    [Fact]
    public async Task GetIndexAsync_Should_Take_Correct_Number_Of_Records()
    {
        var (context, scope) = _fixture.CreateTransactionalContext();
        using (scope)
        {
            var absences = Enumerable.Range(1, 10).Select(i => new Absence
            {
                Name = $"Absence {i}",
                StartDate = DateTime.UtcNow.Date.AddDays(i),
                EndDate = DateTime.UtcNow.Date.AddDays(i + 1),
                Reason = $"Reason {i}"
            }).ToList();

            context.Absences.AddRange(absences);
            await context.SaveChangesAsync();

            var service = new AbsencesService(context);
            var request = new QueryRequest.SkipTake { Skip = 0, Take = 3 };

            var result = await service.GetIndexAsync(request);

            result.IsSuccess.ShouldBeTrue();
            result.Value.Absences.Count().ShouldBe(3);
        }
    }
}
