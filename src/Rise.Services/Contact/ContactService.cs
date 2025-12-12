namespace Rise.Services.Contact;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Rise.Persistence;
using Rise.Shared.Common;
using Rise.Shared.Contact;

 /// <summary>
/// Service for managing contact information.
/// </summary>
public class ContactService(ApplicationDbContext dbContext) : IContactService
{
    /// <summary>
    /// Retrieves a filtered and paginated list of contacts.
    /// Supports searching by type and name, and sorting.
    /// </summary>
    /// <param name="request">QueryRequest.SkipTake with SearchTerm, OrderBy, Skip and Take</param>
    /// <param name="ctx">CancellationToken to cancel the operation</param>
    /// <returns>Result with ContactResponse.Index containing the list of contacts</returns>
    public async Task<Result<ContactResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
   
    {
        var query = dbContext.Contacts.AsQueryable();


        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(n => n.Type.Contains(request.SearchTerm)
                                     || n.Name.Contains(request.SearchTerm));
        }
        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            query = request.OrderDescending
                ? query.OrderByDescending(e => EF.Property<object>(e, request.OrderBy))
                : query.OrderBy(e => EF.Property<object>(e, request.OrderBy));
        }

        else
        {
            query = query.OrderBy(p => p.Type);
        }

        var contact = await query.AsNoTracking()
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(c => new ContactDto.Index
            {
                Id = c.Id,
                Type = c.Type,
                Name = c.Name,
                Email = c.Email,
                ContactPerson = c.ContactPerson,
                PhoneNumber = c.PhoneNumber,
                Campusses = c.Campusses,
            }).ToListAsync(ctx);



        return Result.Success(new ContactResponse.Index
        {
            Contact = contact,
        }
        );
    }
}

