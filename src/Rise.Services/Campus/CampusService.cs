using System.Globalization;
using System.Text.Json;
using Rise.Persistence;
using Rise.Shared.Campus;
using Rise.Shared.Common;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Campus;

namespace Rise.Services.Campus;

// <summary>
// Service for campus.
// </summary>
// <param name="dbContext"></param>
public class CampusService(ApplicationDbContext dbContext) : ICampusService
{

    private readonly string _mockFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Campus", "MockData", "campus.json");
    public async Task<Result<CampusResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var entities = await dbContext.Campuses
            .AsNoTracking()
            .Include(c => c.Buildings)
            .Skip(request.Skip)
            .Take(request.Take)
            .ToListAsync(ctx); 

        var campuses = entities.Select(c => new CampusDto.Index
        {
            Id = c.Id,
            Name = c.Name,
            Street = c.Street,
            HouseNumber = c.HouseNumber,
            City = c.City,
            PostalCode = c.PostalCode,
            ContactPhone = c.ContactPhone,
            Description = c.Description,
            Facilities = c.Facilities,
            Latitude = c.Latitude,
            Longitude = c.Longitude,
            Buildings = c.Buildings.Select(b => new BuildingDto.Index
            {
                Id = b.Id,
                CampusId = b.CampusId,
                Name = b.Name,
                Address = b.Address,
                Type = b.Type,
                Latitude = b.Latitude,
                BuildingCode = b.BuildingCode,
                Longitude = b.Longitude
            }).ToList()
        }).ToList();

        return Result.Success(new CampusResponse.Index { Campuses = campuses });
    }

    public async Task<Result<CampusResponse.Get>> GetCampusByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entitie = await dbContext.Campuses
            .AsNoTracking()
            .Include(c => c.Buildings)
            .Where(c => c.Id.Equals(id))
            .FirstOrDefaultAsync(ct);
        if (entitie == null)
            return Result<CampusResponse.Get>.NotFound($"No Campus found with id {id}");
        var campus =new CampusDto.Index
            {
                Id = entitie.Id,
                Name = entitie.Name,
                Street = entitie.Street,
                HouseNumber = entitie.HouseNumber,
                City = entitie.City,
                PostalCode = entitie.PostalCode,
                ContactPhone = entitie.ContactPhone,
                Description = entitie.Description,
                Facilities = entitie.Facilities,
                Latitude = entitie.Latitude,
                Longitude = entitie.Longitude,
                Buildings = entitie.Buildings.Select(b => new BuildingDto.Index
                {
                    Id = b.Id,
                    CampusId = entitie.Id,
                    Name = b.Name,
                    Address = b.Address,
                    Type = b.Type,
                    Latitude = b.Latitude,
                    BuildingCode = b.BuildingCode,
                    Longitude = b.Longitude
                }).ToList()
            };
        
        
        var response = new CampusResponse.Get { Campus = campus };
        return Result.Success(response);
    }
    public async Task<Result<BuildingResponse.Get>> GetBuildingByBuildingCodeAsync(string code, CancellationToken ct = default)
    {
        var query = dbContext.Buildings.AsQueryable();
        var building = await query.AsNoTracking()
            .Where(b => b.BuildingCode.Equals(code))
            .Select(b => new BuildingDto.Index
            {
                Id = b.Id,
                Name = b.Name,
                Type = b.Type,
                Address = b.Address,
                Latitude = b.Latitude,
                Longitude = b.Longitude,
                BuildingCode = b.BuildingCode,
                CampusId = b.CampusId
            }).FirstOrDefaultAsync(ct);
        
        if (building == null)
            return Result<BuildingResponse.Get>.NotFound($"No Building found with buildingcode {code}");
        var response = new BuildingResponse.Get { Building = building };
        return Result.Success(response);
    }

}
