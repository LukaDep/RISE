using Rise.Shared.Contact;

namespace Rise.Server.Endpoints.Contact
{
    public class Get(IContactService contactService) : EndpointWithoutRequest<Result<ContactResponse.Get>>
    {
        public override void Configure()
        {
            Get("/api/contact/{id}");
            AllowAnonymous();
        }

        public override async Task<Result<ContactResponse.Get>> ExecuteAsync(CancellationToken ct)
        {
            var id = Route<int>("id");
            return await contactService.GetByIdAsync(id, ct);
        }
    }
}
