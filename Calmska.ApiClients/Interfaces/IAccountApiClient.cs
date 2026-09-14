using Calmska.Application.DTO;
using Calmska.Domain.Common;

namespace Calmska.ApiClients.Interfaces
{
    public interface IAccountApiClient : IApiClient<AccountDTO> 
    {
        /// <summary>
        /// Check if user with specific email and password exists.
        /// </summary>
        /// <param name="criteria">The criteria for searching object.</param>
        /// <returns>True if found, or false if not.</returns>
        Task<OperationResultT<bool>> LoginAsync(LoginDTO criteria, CancellationToken token);
    }
}
