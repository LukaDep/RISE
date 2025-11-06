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

    private readonly string _mockFilePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Rise.Services", "Campus", "MockData", "campus.json");
    public async Task<Result<CampusResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var query = dbContext.Campuses.AsQueryable();

        var campuses = await query.AsNoTracking()
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(c => new CampusDto.Index
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
                Buildings = (c.Buildings ?? Array.Empty<Building>()).Select(b => new BuildingDto.Index
                    {
                        Id = b.Id,
                        CampusId = c.Id,
                        Name = b.Name,
                        Address = b.Address,
                        Type = b.Type,
                        Latitude = b.Latitude,
                        Longitude = b.Longitude
                    }).ToList()
                }).ToListAsync(ctx);

        return Result.Success(new CampusResponse.Index
            {
                Campuses = campuses
            }
        );
    }

    public async Task<Result<CampusResponse.Get>> GetCampusByIdAsync(Guid id, CancellationToken ct = default)
    {
        var query = dbContext.Campuses.AsQueryable();
        var campus = await query.AsNoTracking()
            .Where(c => c.Id.Equals(id))
            .Select(c => new CampusDto.Index
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
                Buildings = (c.Buildings ?? Array.Empty<Building>()).Select(b => new BuildingDto.Index
                {
                    Id = b.Id,
                    CampusId = c.Id,
                    Name = b.Name,
                    Address = b.Address,
                    Type = b.Type,
                    Latitude = b.Latitude,
                    Longitude = b.Longitude
                }).ToList()
            }).FirstOrDefaultAsync(ct);
        
        if (campus == null)
            return Result<CampusResponse.Get>.NotFound($"No Campus found with id {id}");
        var response = new CampusResponse.Get { Campus = campus };
        return Result.Success(response);
    }
    public async Task<Result<BuildingResponse.Get>> GetBuildingByIdAsync(Guid buildingId, CancellationToken ct = default)
    {
        var query = dbContext.Buildings.AsQueryable();
        var building = await query.AsNoTracking()
            .Where(b => b.Id.Equals(buildingId))
            .Select(b => new BuildingDto.Index
            {
                Id = b.Id,
                Name = b.Name,
                Type = b.Type,
                Address = b.Address,
                Latitude = b.Latitude,
                Longitude = b.Longitude,
                CampusId = b.CampusId
            }).FirstOrDefaultAsync(ct);
        
        if (building == null)
            return Result<BuildingResponse.Get>.NotFound($"No Building found with id {buildingId}");
        var response = new BuildingResponse.Get { Building = building };
        return Result.Success(response);
    }

}