using Rise.Shared.Contact;
using Rise.Shared.Common;
namespace Rise.Server.Endpoints.Contact
{
    /// <summary>
    /// List all contacts.
    /// </summary>
    /// <param name="contactService">The contact service.</param>
    public class Index(IContactService contactService) : Endpoint<QueryRequest.SkipTake, Result<ContactResponse.Index>>
    {
        /// <summary>
        /// Configures the endpoint route and authorization.
        /// </summary>
        public override void Configure()
        {
            Get("/api/contacts");
            AllowAnonymous();
        }

        /// <summary>
        /// Retrieves a paginated list of all contacts.
        /// </summary>
        /// <param name="req">The pagination request containing skip and take values.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A result containing the list of contacts.</returns>
        public override Task<Result<ContactResponse.Index>> ExecuteAsync(QueryRequest.SkipTake req, CancellationToken ct)
        {
            return contactService.GetIndexAsync(req, ct);
        }
    }
}
