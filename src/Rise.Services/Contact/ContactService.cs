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

namespace Rise.Services.Contact
{
    internal class ContactService(ApplicationDbContext dbContext) : IContactService
    {
        private readonly string _mockFilePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Rise.Services", "Contact", "MockData", "contact.json");
        public async Task<Result<ContactResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
        {
            // if (!File.Exists(_mockFilePath))
            //     return Result<ContactResponse.Index>.NotFound($"Mock data file not found at: {_mockFilePath}");
            // var json = await File.ReadAllTextAsync(_mockFilePath, ctx);
            //
            // var query = JsonSerializer.Deserialize<List<ContactDto.Index>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            var query = dbContext.Contacts.AsQueryable();
            
            var typeFilter = request.Filters["Type"]?.ToString() ?? "";
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
            if (!string.IsNullOrWhiteSpace(typeFilter))
            {
                query = query.Where(n => n.Type.Equals(typeFilter, StringComparison.CurrentCultureIgnoreCase)).ToList();
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
                    phoneNumber = c.PhoneNumber,
                    Campusses = c.Campusses,
                }).ToListAsync(ctx);
                


            return Result.Success(new ContactResponse.Index
            {
                Contact = contact,
            }
            );
        }
    }
}
