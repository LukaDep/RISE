using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.Common;
using Rise.Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Rise.Services.Events
{
    public class EventService(ApplicationDbContext dbContext) : IEventService
    {
        public async Task<Result<EventResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
        {

            var query = dbContext.Events.AsQueryable();
            string? typeFilter = "";

            // Filter only future events (starting from today)
            var today = DateTime.Today;
            query = query.Where(e => e.StartDateTime.Date >= today);

            if (request.Filters.ContainsKey("Type"))
            {
                typeFilter = request.Filters["Type"]?.ToString();
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(n => n.Type.Contains(request.SearchTerm)
                                         || n.Title.Contains(request.SearchTerm));
            }
            if (!string.IsNullOrWhiteSpace(request.OrderBy))
            {
                query = request.OrderDescending
                    ? query.OrderByDescending(e => EF.Property<object>(e, request.OrderBy))
                    : query.OrderBy(e => EF.Property<object>(e, request.OrderBy));
            }
            if (!string.IsNullOrWhiteSpace(typeFilter))
            {
                var lowerFilter = typeFilter.ToLower();
                query = query.Where(n => n.Type.ToLower() == lowerFilter);
            }

            query = query.OrderBy(e => e.StartDateTime).ThenBy(p => p.Type);

            var events = await query.AsNoTracking()
                .Skip(request.Skip)
                .Take(request.Take)
                .Select(e => new EventDTO.Index
                {
                    Type = e.Type,
                    Name = e.Title,
                    StartDateTime = e.StartDateTime,
                    EndDateTime = e.EndDateTime,
                    Location = e.Location,
                    RegistrationLink = e.RegistrationLink,
                    Description = e.Description
                }).ToListAsync(ctx);



            return Result.Success(new EventResponse.Index
            {
                Event = events,
            }
            );
        }

    }
}
