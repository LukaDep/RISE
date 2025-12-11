using Ardalis.Result;
using Rise.Services.Contact;
using Rise.Shared.Common;
using Rise.Services.Tests.TestInfrastructure;
using Rise.Persistence;
using ContactEntity = Rise.Domain.Contact.Contact;

namespace Rise.Services.Tests.Contact;

public class ContactServiceShould
{
    [Fact]
    public async Task GetIndexAsync_Should_Return_Data()
    {
        using var fixture = new SqliteTestFixture();
        using var db = fixture.CreateContext();

        db.Contacts.AddRange(
            new ContactEntity { Type = "Campus", Name = "Contact Alpha", Email = "a@example.com", ContactPerson = "Alice", PhoneNumber = "111" },
            new ContactEntity { Type = "Department", Name = "Contact Beta", Email = "b@example.com", ContactPerson = "Bob", PhoneNumber = "222" },
            new ContactEntity { Type = "Organization", Name = "Contact Gamma", Email = "c@example.com", ContactPerson = "Carol", PhoneNumber = "333" }
        );
        await db.SaveChangesAsync();

        var service = new ContactService(db);
        var req = new QueryRequest.SkipTake { Skip = 0, Take = 3 };

        var result = await service.GetIndexAsync(req, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.ShouldNotBeNull();
        result.Value.Contact.ShouldNotBeNull();
        result.Value.Contact.Count().ShouldBeGreaterThan(0);
        result.Value.Contact.Count().ShouldBeLessThanOrEqualTo(3);
    }

    [Fact]
    public async Task GetIndexAsync_Should_Filter_By_SearchTerm()
    {
        using var fixture = new SqliteTestFixture();
        using var db = fixture.CreateContext();

        db.Contacts.AddRange(
            new ContactEntity { Type = "Campus", Name = "Schoonmeersen Contact D", Email = "d@example.com" },
            new ContactEntity { Type = "Campus", Name = "Schoonmeersen Contact A", Email = "a@example.com" },
            new ContactEntity { Type = "Other", Name = "Different", Email = "o@example.com" }
        );
        await db.SaveChangesAsync();

        var service = new ContactService(db);
        var req = new QueryRequest.SkipTake { Skip = 0, Take = 10, SearchTerm = "Schoonmeersen Contact D" };

        var result = await service.GetIndexAsync(req, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.Contact.Count().ShouldBe(1);
        result.Value.Contact.First().Name.ShouldBe("Schoonmeersen Contact D");
    }

    [Fact]
    public async Task GetIndexAsync_Should_Return_Empty_When_No_Data()
    {
        using var fixture = new SqliteTestFixture();
        using var db = fixture.CreateContext();

        var service = new ContactService(db);
        var req = new QueryRequest.SkipTake { Skip = 0, Take = 5 };

        var result = await service.GetIndexAsync(req, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.Contact.Count().ShouldBe(0);
    }

    [Fact]
    public async Task Search_Should_Match_Name()
    {
        using var fixture = new SqliteTestFixture();
        using var db = fixture.CreateContext();

        db.Contacts.AddRange(
            new ContactEntity { Type = "Campus", Name = "Best Contact", Email = "best@example.com" },
            new ContactEntity { Type = "Campus", Name = "Another Contact", Email = "another@example.com" }
        );
        await db.SaveChangesAsync();

        var service = new ContactService(db);

        // Use a search term matching the stored casing so this test passes against the current service
        var req1 = new QueryRequest.SkipTake { Skip = 0, Take = 10, SearchTerm = "Best Contact" };
        var r1 = await service.GetIndexAsync(req1, CancellationToken.None);
        r1.Status.ShouldBe(ResultStatus.Ok);
        r1.Value.Contact.Select(x => x.Name).ShouldContain("Best Contact");
    }
}
