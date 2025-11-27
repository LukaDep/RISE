using Ardalis.Result;
using Rise.Shared.Campus;
using Rise.Shared.Common;

namespace Rise.Client.Campus;

public class FakeCampusService : ICampusService
{
    private readonly List<CampusDto.Index> _campuses =
    [
        new CampusDto.Index
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Campus Schoonmeersen",
            Street = "Valentin Vaerwyckweg",
            HouseNumber = "1",
            City = "Gent",
            PostalCode = "9000",
            ContactPhone = "+32 9 243 25 85",
            Description = "Main campus with modern facilities",
            Facilities = new List<string> { "Library", "Cafeteria", "Sports Hall" },
            Latitude = 51.0315,
            Longitude = 3.7089,
            Buildings =
            [
                new BuildingDto.Index
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    CampusId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Building A",
                    Address = "Valentin Vaerwyckweg 1",
                    Type = "Educational",
                    BuildingCode = "A",
                    Latitude = 51.0315,
                    Longitude = 3.7089
                }
            ]
        },
        new CampusDto.Index
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "Campus Mercator",
            Street = "Henleykaai",
            HouseNumber = "84",
            City = "Gent",
            PostalCode = "9000",
            ContactPhone = "+32 9 243 28 00",
            Description = "Campus near the water",
            Facilities = new List<string> { "Computer Lab", "Meeting Rooms" },
            Latitude = 51.0567,
            Longitude = 3.7303,
            Buildings =
            [
                new BuildingDto.Index
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    CampusId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Building M",
                    Address = "Henleykaai 84",
                    Type = "Educational",
                    BuildingCode = "M",
                    Latitude = 51.0567,
                    Longitude = 3.7303
                }
            ]
        },
        new CampusDto.Index
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Name = "Campus Vesalius",
            Street = "Keramiekstraat",
            HouseNumber = "80",
            City = "Gent",
            PostalCode = "9000",
            ContactPhone = "+32 9 243 25 50",
            Description = "Health campus",
            Facilities = new List<string> { "Medical Lab", "Study Rooms" },
            Latitude = 51.0432,
            Longitude = 3.7350,
            Buildings = []
        }
    ];

    public Task<Result<CampusResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var query = _campuses.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            query = query.Where(c =>
                c.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                c.City.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                c.Description.Contains(term, StringComparison.OrdinalIgnoreCase));
        }
        var skip = Math.Max(0, request.Skip);
        var take = Math.Max(0, request.Take);
        var page = query.Skip(skip).Take(take).ToList();

        var response = new CampusResponse.Index { Campuses = page };
        return Task.FromResult(Result.Success(response));
    }

    public Task<Result<CampusResponse.Get>> GetCampusByIdAsync(Guid id, CancellationToken ct = default)
    {
        var campus = _campuses.FirstOrDefault(c => c.Id == id);
        if (campus == null)
            return Task.FromResult(Result<CampusResponse.Get>.NotFound($"No Campus found with id {id}"));

        var response = new CampusResponse.Get { Campus = campus };
        return Task.FromResult(Result.Success(response));
    }

    public Task<Result<BuildingResponse.Get>> GetBuildingByBuildingCodeAsync(string code, CancellationToken ct = default)
    {
        var building = _campuses
            .SelectMany(c => c.Buildings)
            .FirstOrDefault(b => b.BuildingCode == code);

        if (building == null)
            return Task.FromResult(Result<BuildingResponse.Get>.NotFound($"No Building found with buildingcode {code}"));

        var response = new BuildingResponse.Get { Building = building };
        return Task.FromResult(Result.Success(response));
    }
}
