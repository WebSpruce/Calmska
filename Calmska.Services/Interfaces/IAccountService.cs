using Calmska.Models.DTO;
using Calmska.Models.Models;

namespace Calmska.Services.Interfaces
{
    public interface IAccountService : IService<AccountDTO> 
    {
        /// <summary>
        /// Check if user with specific email and password exists.
        /// </summary>
        /// <param name="criteria">The criteria for searching object.</param>
        /// <returns>True if found, or false if not.</returns>
        Task<OperationResultT<bool>> LoginAsync(AccountDTO criteria);
    }
}
