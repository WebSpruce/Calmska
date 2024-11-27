using Calmska.Api.DTO;
using Calmska.Models.Models;

namespace Calmska.Api.Interfaces
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAllAsync();
        Task<IEnumerable<Account>> GetAllByArgumentAsync(AccountDTO account);
        Task<Account?> GetByArgumentAsync(AccountDTO account);
        Task<OperationResult> AddAsync(AccountDTO account);
        Task<OperationResult> UpdateAsync(AccountDTO account);
        Task<OperationResult> DeleteAsync(Account account);
    }
}
