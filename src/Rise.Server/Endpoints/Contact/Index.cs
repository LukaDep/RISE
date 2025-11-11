using Rise.Shared.Contact;
using Rise.Shared.Common;
namespace Rise.Server.Endpoints.Contact
{
    public class Index(IContactService contactService) : Endpoint<QueryRequest.SkipTake, Result<ContactResponse.Index>>
    {
        public override void Configure()
        {
            Get("/api/contacts");
            AllowAnonymous();
        }
        public override Task<Result<ContactResponse.Index>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
        {
            return contactService.GetIndexAsync(req, ct);
        }
    }
}
