using Ardalis.Result;
using Rise.Services.Resto;
using Rise.Shared.Common;
using Rise.Services.Tests.TestInfrastructure;
using Rise.Persistence;
using RestoEntity = Rise.Domain.Restos.Resto;

namespace Rise.Services.Tests.Resto;

public class RestoServiceShould
{
    [Fact]
    public async Task GetIndexAsync_Should_Return_Data()
    {
        using var fixture = new SqliteTestFixture();
        using var db = fixture.CreateContext();

        db.Restos.AddRange(
            new RestoEntity
            {
                Name = "Alpha",
                Description = "Cozy place",
                BuildingId = Guid.NewGuid(),
                OpeningHours = new Dictionary<DayOfWeek, string> { [DateTimeOffset.Now.DayOfWeek] = "08:00-18:00" }
            },
            new RestoEntity
            {
                Name = "Beta",
                Description = "Great coffee",
                BuildingId = Guid.NewGuid(),
                OpeningHours = new Dictionary<DayOfWeek, string> { [DateTimeOffset.Now.DayOfWeek] = "09:00-17:00" }
            },
            new RestoEntity
            {
                Name = "Gamma",
                Description = "Snacks & more",
                BuildingId = Guid.NewGuid(),
                OpeningHours = new Dictionary<DayOfWeek, string> { [DateTimeOffset.Now.DayOfWeek] = "10:00-16:00" }
            }
        );
        await db.SaveChangesAsync();

        var service = new RestoService(db);
        var req = new QueryRequest.SkipTake { Skip = 0, Take = 3 };

        var result = await service.GetIndexAsync(req, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.ShouldNotBeNull();
        result.Value.Restos.ShouldNotBeNull();
        result.Value.Restos.Count().ShouldBeGreaterThan(0);
        result.Value.Restos.Count().ShouldBeLessThanOrEqualTo(3);
    }

    [Fact]
    public async Task GetIndexAsync_Should_Filter_By_SearchTerm()
    {
        using var fixture = new SqliteTestFixture();
        using var db = fixture.CreateContext();

        db.Restos.AddRange(
            new RestoEntity { Name = "Resto Schoonmeersen D", Description = "Main hall", BuildingId = Guid.NewGuid(), OpeningHours = new() },
            new RestoEntity { Name = "Resto Schoonmeersen A", Description = "Annex", BuildingId = Guid.NewGuid(), OpeningHours = new() },
            new RestoEntity { Name = "Other", Description = "Different", BuildingId = Guid.NewGuid(), OpeningHours = new() }
        );
        await db.SaveChangesAsync();

        var service = new RestoService(db);
        var req = new QueryRequest.SkipTake { Skip = 0, Take = 10, SearchTerm = "Resto Schoonmeersen D" };

        var result = await service.GetIndexAsync(req, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.Restos.Count().ShouldBe(1);
        result.Value.Restos.First().Name.ShouldBe("Resto Schoonmeersen D");
    }

    [Fact]
    public async Task GetIndexAsync_Should_Return_Empty_When_No_Data()
    {
        using var fixture = new SqliteTestFixture();
        using var db = fixture.CreateContext();

        var service = new RestoService(db);
        var req = new QueryRequest.SkipTake { Skip = 0, Take = 5 };

        var result = await service.GetIndexAsync(req, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.Restos.Count().ShouldBe(0);
    }

    [Fact]
    public async Task IsCurrentlyOpen_Should_Be_True_When_Today_All_Day()
    {
        using var fixture = new SqliteTestFixture();
        using var db = fixture.CreateContext();

        var today = DateTimeOffset.Now.DayOfWeek;

        var resto = new RestoEntity
        {
            Name = "Test Resto Open All Day",
            BuildingId = Guid.NewGuid(),
            OpeningHours = new Dictionary<DayOfWeek, string> { [today] = "00:00-23:59" }
        };
        db.Restos.Add(resto);
        await db.SaveChangesAsync();

        var service = new RestoService(db);
        var req = new QueryRequest.SkipTake { Skip = 0, Take = 10 };
        var result = await service.GetIndexAsync(req, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.Restos.ShouldNotBeEmpty();
        result.Value.Restos.First(r => r.Id == resto.Id).IsCurrentlyOpen.ShouldBeTrue();
    }

    [Fact]
    public async Task IsCurrentlyOpen_Should_Be_False_When_No_Hours()
    {
        using var fixture = new SqliteTestFixture();
        using var db = fixture.CreateContext();

        var resto = new RestoEntity
        {
            Name = "Test Resto Closed",
            BuildingId = Guid.NewGuid(),
            OpeningHours = new Dictionary<DayOfWeek, string>()
        };
        db.Restos.Add(resto);
        await db.SaveChangesAsync();

        var service = new RestoService(db);
        var req = new QueryRequest.SkipTake { Skip = 0, Take = 10 };
        var result = await service.GetIndexAsync(req, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.Restos.First(r => r.Id == resto.Id).IsCurrentlyOpen.ShouldBeFalse();
    }

    [Fact]
    public async Task Search_Should_Match_Description_Ignoring_Case()
    {
        using var fixture = new SqliteTestFixture();
        using var db = fixture.CreateContext();

        db.Restos.AddRange(
            new RestoEntity { Name = "Cafe Alpha", Description = "best sandwiches", BuildingId = Guid.NewGuid(), OpeningHours = new() },
            new RestoEntity { Name = "Cafe Beta", Description = "another place", BuildingId = Guid.NewGuid(), OpeningHours = new() }
        );
        await db.SaveChangesAsync();

        var service = new RestoService(db);

        var req1 = new QueryRequest.SkipTake { Skip = 0, Take = 10, SearchTerm = "SANDWICHES" };
        var r1 = await service.GetIndexAsync(req1, CancellationToken.None);
        r1.Status.ShouldBe(ResultStatus.Ok);
        r1.Value.Restos.Select(x => x.Name).ShouldContain("Cafe Alpha");
    }

    [Fact]
    public async Task OpeningHours_Should_Handle_Overnight_Range_Ending_Next_Day()
    {
        using var fixture = new SqliteTestFixture();
        using var db = fixture.CreateContext();

        var today = DateTimeOffset.Now.DayOfWeek;

        var resto = new RestoEntity
        {
            Name = "Night Cafe",
            BuildingId = Guid.NewGuid(),
            OpeningHours = new Dictionary<DayOfWeek, string> { [today] = "23:00-01:00" }
        };
        db.Restos.Add(resto);
        await db.SaveChangesAsync();

        var service = new RestoService(db);
        var req = new QueryRequest.SkipTake { Skip = 0, Take = 10 };
        var result = await service.GetIndexAsync(req, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.Restos.Any(r => r.Id == resto.Id).ShouldBeTrue();
    }
}
