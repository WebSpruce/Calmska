using Calmska.Models.DTO;
using Calmska.Models.Models;

namespace Calmska.Api.Interfaces
{
    public interface IAccountRepository : IRepository<Account, AccountDTO>
    {
        Task<bool> LoginAsync(AccountDTO filter);
    }
}
