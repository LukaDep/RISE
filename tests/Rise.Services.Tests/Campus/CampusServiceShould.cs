using Rise.Domain.Campus;
using Rise.Services.Campus;
using Rise.Services.Tests.TestInfrastructure;
using Rise.Shared.Common;
using Shouldly;
using Xunit;

namespace Rise.Services.Tests.Campus;

public class CampusServiceShould : IDisposable
{
    private readonly SqliteTestFixture _fixture;

    public CampusServiceShould()
    {
        _fixture = new SqliteTestFixture();
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }

    [Fact]
    public async Task GetIndexAsync_Should_Return_Campuses()
    {
        var (context, scope) = _fixture.CreateTransactionalContext();
        using (scope)
        {
            var campus1 = new Rise.Domain.Campus.Campus
            {
                Name = "Campus Alpha",
                Street = "Main Street",
                HouseNumber = "1",
                City = "TestCity",
                PostalCode = "1000",
                ContactPhone = "+32 9 123 45 67",
                Description = "Test campus 1",
                Facilities = new List<string> { "Library" },
                Latitude = 51.0,
                Longitude = 3.7
            };
            var campus2 = new Rise.Domain.Campus.Campus
            {
                Name = "Campus Beta",
                Street = "Second Street",
                HouseNumber = "2",
                City = "TestCity",
                PostalCode = "2000",
                ContactPhone = "+32 9 234 56 78",
                Description = "Test campus 2",
                Facilities = new List<string> { "Cafeteria" },
                Latitude = 51.1,
                Longitude = 3.8
            };

            context.Campuses.AddRange(campus1, campus2);
            await context.SaveChangesAsync();

            var service = new CampusService(context);
            var request = new QueryRequest.SkipTake { Skip = 0, Take = 10 };
            var result = await service.GetIndexAsync(request);
            result.IsSuccess.ShouldBeTrue();
            result.Value.Campuses.ShouldNotBeNull();
            result.Value.Campuses.Count().ShouldBe(2);
            result.Value.Campuses.Select(c => c.Name).ShouldContain("Campus Alpha");
            result.Value.Campuses.Select(c => c.Name).ShouldContain("Campus Beta");
        }
    }

    [Fact]
    public async Task GetIndexAsync_Should_Apply_Pagination()
    {
        var (context, scope) = _fixture.CreateTransactionalContext();
        using (scope)
        {
            for (int i = 0; i < 5; i++)
            {
                context.Campuses.Add(new Rise.Domain.Campus.Campus
                {
                    Name = $"Campus {i}",
                    Street = "Test Street",
                    HouseNumber = i.ToString(),
                    City = "TestCity",
                    PostalCode = "1000",
                    ContactPhone = "+32 9 123 45 67",
                    Description = $"Campus {i}",
                    Facilities = new List<string>(),
                    Latitude = 51.0,
                    Longitude = 3.7
                });
            }
            await context.SaveChangesAsync();

            var service = new CampusService(context);
            var request = new QueryRequest.SkipTake { Skip = 2, Take = 2 };
            var result = await service.GetIndexAsync(request);
            result.IsSuccess.ShouldBeTrue();
            result.Value.Campuses.Count().ShouldBe(2);
        }
    }

    [Fact]
    public async Task GetIndexAsync_Should_Include_Buildings()
    {
        var (context, scope) = _fixture.CreateTransactionalContext();
        using (scope)
        {
            var campus = new Rise.Domain.Campus.Campus
            {
                Name = "Test Campus",
                Street = "Test Street",
                HouseNumber = "1",
                City = "TestCity",
                PostalCode = "1000",
                ContactPhone = "+32 9 123 45 67",
                Description = "Test",
                Facilities = new List<string>(),
                Latitude = 51.0,
                Longitude = 3.7
            };
            context.Campuses.Add(campus);
            await context.SaveChangesAsync();

            var building = new Building
            {
                CampusId = campus.Id,
                Name = "Building A",
                Address = "Test Address",
                Type = "Educational",
                BuildingCode = "A",
                Latitude = 51.0,
                Longitude = 3.7
            };
            context.Buildings.Add(building);
            await context.SaveChangesAsync();

            var service = new CampusService(context);
            var request = new QueryRequest.SkipTake { Skip = 0, Take = 10 };
            var result = await service.GetIndexAsync(request);
            result.IsSuccess.ShouldBeTrue();
            var campusDto = result.Value.Campuses.First();
            campusDto.Buildings.ShouldNotBeNull();
            campusDto.Buildings.Count().ShouldBe(1);
            campusDto.Buildings.First().Name.ShouldBe("Building A");
        }
    }

    [Fact]
    public async Task GetCampusByIdAsync_Should_Return_Campus()
    {
        var (context, scope) = _fixture.CreateTransactionalContext();
        using (scope)
        {
            var campus = new Rise.Domain.Campus.Campus
            {
                Name = "Test Campus",
                Street = "Test Street",
                HouseNumber = "1",
                City = "TestCity",
                PostalCode = "1000",
                ContactPhone = "+32 9 123 45 67",
                Description = "Test",
                Facilities = new List<string> { "Library", "Gym" },
                Latitude = 51.0,
                Longitude = 3.7
            };
            context.Campuses.Add(campus);
            await context.SaveChangesAsync();

            var service = new CampusService(context);
            var result = await service.GetCampusByIdAsync(campus.Id);
            result.IsSuccess.ShouldBeTrue();
            result.Value.Campus.ShouldNotBeNull();
            result.Value.Campus.Name.ShouldBe("Test Campus");
            result.Value.Campus.Facilities.ShouldContain("Library");
        }
    }

    [Fact]
    public async Task GetCampusByIdAsync_Should_Return_NotFound_When_Campus_DoesNotExist()
    {
        var (context, scope) = _fixture.CreateTransactionalContext();
        using (scope)
        {
            var service = new CampusService(context);
            var nonExistentId = Guid.NewGuid();
            var result = await service.GetCampusByIdAsync(nonExistentId);
            result.IsSuccess.ShouldBeFalse();
            result.Status.ShouldBe(Ardalis.Result.ResultStatus.NotFound);
        }
    }

    [Fact]
    public async Task GetBuildingByBuildingCodeAsync_Should_Return_Building()
    {
        var (context, scope) = _fixture.CreateTransactionalContext();
        using (scope)
        {
            var campus = new Rise.Domain.Campus.Campus
            {
                Name = "Test Campus",
                Street = "Test Street",
                HouseNumber = "1",
                City = "TestCity",
                PostalCode = "1000",
                ContactPhone = "+32 9 123 45 67",
                Description = "Test",
                Facilities = new List<string>(),
                Latitude = 51.0,
                Longitude = 3.7
            };
            context.Campuses.Add(campus);
            await context.SaveChangesAsync();

            var building = new Building
            {
                CampusId = campus.Id,
                Name = "Building A",
                Address = "Test Address",
                Type = "Educational",
                BuildingCode = "BLDG-A",
                Latitude = 51.0,
                Longitude = 3.7
            };
            context.Buildings.Add(building);
            await context.SaveChangesAsync();

            var service = new CampusService(context);
            var result = await service.GetBuildingByBuildingCodeAsync("BLDG-A");
            result.IsSuccess.ShouldBeTrue();
            result.Value.Building.ShouldNotBeNull();
            result.Value.Building.Name.ShouldBe("Building A");
            result.Value.Building.BuildingCode.ShouldBe("BLDG-A");
        }
    }

    [Fact]
    public async Task GetBuildingByBuildingCodeAsync_Should_Return_NotFound_When_Building_DoesNotExist()
    {
        var (context, scope) = _fixture.CreateTransactionalContext();
        using (scope)
        {
            var service = new CampusService(context);
            var result = await service.GetBuildingByBuildingCodeAsync("NONEXISTENT");
            result.IsSuccess.ShouldBeFalse();
            result.Status.ShouldBe(Ardalis.Result.ResultStatus.NotFound);
        }
    }

    [Fact]
    public async Task GetIndexAsync_Should_Return_Empty_List_When_No_Campuses()
    {
        var (context, scope) = _fixture.CreateTransactionalContext();
        using (scope)
        {
            var service = new CampusService(context);
            var request = new QueryRequest.SkipTake { Skip = 0, Take = 10 };
            var result = await service.GetIndexAsync(request);
            result.IsSuccess.ShouldBeTrue();
            result.Value.Campuses.ShouldBeEmpty();
        }
    }
}
