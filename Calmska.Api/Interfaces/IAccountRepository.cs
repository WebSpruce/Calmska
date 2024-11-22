using Calmska.Models.Models;

namespace Calmska.Api.Interfaces
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAllAsync();
        Task<IEnumerable<Account>> GetAllByArgumentAsync(Account account);
        Task<Account?> GetByArgumentAsync(Account account);
        Task<bool> AddAsync(Account account);
        Task<bool> UpdateAsync(Account account);
        Task<bool> DeleteAsync(Account account);
    }
}
