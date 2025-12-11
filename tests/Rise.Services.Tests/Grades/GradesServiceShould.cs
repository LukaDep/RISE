using System.Security.Claims;
using Ardalis.Result;
using Rise.Services.Tests.TestInfrastructure;
using Rise.Services.Tests.Fakers;
using Rise.Persistence;
using Rise.Domain.Grades;
using Rise.Services.Grades;
using Rise.Shared.Common;
using Rise.Shared.Grades;

namespace Rise.Services.Tests.Grades;

public class GradesServiceShould
{
    [Fact]
    public async Task GetIndexAsync_Should_Return_Data_For_User()
    {
        using var fixture = new SqliteTestFixture();
        using var db = fixture.CreateContext();

        var userId = Guid.NewGuid().ToString();

        db.Grades.AddRange(
            new Grade { Name = "Test1", ActivityType = "Exam", CourseName = "Math", Year = "2024", Semester = 1, Score = 90, MaxPoints = 100, Date = DateTime.UtcNow, UserId = userId },
            new Grade { Name = "Test2", ActivityType = "Exam", CourseName = "Physics", Year = "2024", Semester = 1, Score = 80, MaxPoints = 100, Date = DateTime.UtcNow.AddDays(-1), UserId = userId }
        );
        await db.SaveChangesAsync();

        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }));
        var session = new FakeSessionContextProvider(principal);

        var service = new GradesService(db, session);

        var req = new QueryRequest.SkipTake { Skip = 0, Take = 10 };

        var result = await service.GetIndexAsync(req, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.ShouldNotBeNull();
        result.Value.Grades.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task GetIndexAsync_Should_Filter_By_SearchTerm()
    {
        using var fixture = new SqliteTestFixture();
        using var db = fixture.CreateContext();

        var userId = Guid.NewGuid().ToString();

        db.Grades.AddRange(
            new Grade { Name = "Algebra Final", ActivityType = "Exam", CourseName = "Math", Year = "2024", Semester = 1, Date = DateTime.UtcNow, UserId = userId },
            new Grade { Name = "Chem Lab", ActivityType = "Lab", CourseName = "Chemistry", Year = "2024", Semester = 1, Date = DateTime.UtcNow, UserId = userId }
        );
        await db.SaveChangesAsync();

        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }));
        var session = new FakeSessionContextProvider(principal);
        var service = new GradesService(db, session);

        var req = new QueryRequest.SkipTake { Skip = 0, Take = 10, SearchTerm = "Algebra" };
        var result = await service.GetIndexAsync(req, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.Grades.Count().ShouldBe(1);
        result.Value.Grades.First().Name.ShouldContain("Algebra");
    }

    [Fact]
    public async Task GetGradeByIdAsync_Should_Return_Grade_When_Owned()
    {
        using var fixture = new SqliteTestFixture();
        using var db = fixture.CreateContext();

        var userId = Guid.NewGuid().ToString();
        var grade = new Grade { Name = "Single", ActivityType = "Exam", CourseName = "CS", Year = "2025", Semester = 1, Date = DateTime.UtcNow, UserId = userId };
        db.Grades.Add(grade);
        await db.SaveChangesAsync();

        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }));
        var session = new FakeSessionContextProvider(principal);
        var service = new GradesService(db, session);

        var result = await service.GetGradeByIdAsync(grade.Id, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.ShouldNotBeNull();
        result.Value.Grade.Id.ShouldBe(grade.Id);
    }

    [Fact]
    public async Task GetGradeByIdAsync_Should_Return_NotFound_When_Other_User()
    {
        using var fixture = new SqliteTestFixture();
        using var db = fixture.CreateContext();

        var ownerId = Guid.NewGuid().ToString();
        var otherId = Guid.NewGuid().ToString();

        var grade = new Grade { Name = "OtherUser", ActivityType = "Exam", CourseName = "CS", Year = "2025", Semester = 1, Date = DateTime.UtcNow, UserId = ownerId };
        db.Grades.Add(grade);
        await db.SaveChangesAsync();

        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, otherId) }));
        var session = new FakeSessionContextProvider(principal);
        var service = new GradesService(db, session);

        var result = await service.GetGradeByIdAsync(grade.Id, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Methods_Should_Handle_Unauthenticated_User()
    {
        using var fixture = new SqliteTestFixture();
        using var db = fixture.CreateContext();

        // create a grade but no authenticated user in session
        var grade = new Grade { Name = "Anon", ActivityType = "Exam", CourseName = "CS", Year = "2025", Semester = 1, Date = DateTime.UtcNow, UserId = "some-user" };
        db.Grades.Add(grade);
        await db.SaveChangesAsync();

        var principal = new ClaimsPrincipal(); // no identity
        var session = new FakeSessionContextProvider(principal);
        var service = new GradesService(db, session);

        var idx = await service.GetIndexAsync(new QueryRequest.SkipTake { Skip = 0, Take = 10 }, CancellationToken.None);
        idx.Status.ShouldBe(ResultStatus.Ok);
        idx.Value.Grades.ShouldBeEmpty();

        var single = await service.GetGradeByIdAsync(grade.Id, CancellationToken.None);
        single.Status.ShouldBe(ResultStatus.Unauthorized);
    }
}
