using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rise.Shared.Common;

namespace Rise.Shared.Contact
{
    public interface IContactService
    {
        Task<Result<ContactResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default);
    }
}
