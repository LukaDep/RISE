using Rise.Shared.Common;

namespace Rise.Shared.Contact;

    /// <summary>
    /// Service interface for managing contact information.
    /// </summary>
    public interface IContactService
    {
        /// <summary>
        /// Retrieves a filtered and paginated list of contacts.
        /// Supports searching by type and name, and sorting.
        /// </summary>
        /// <param name="request">QueryRequest.SkipTake with SearchTerm, OrderBy, Skip and Take</param>
        /// <param name="ctx">CancellationToken to cancel the operation</param>
        /// <returns>Result with ContactResponse.Index containing the list of contacts</returns>
    Task<Result<ContactResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default);
    }


