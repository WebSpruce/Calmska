using Calmska.Models.DTO;
using Calmska.Models.Models;
using Calmska.Services.Interfaces;

namespace Calmska.Services.Services
{
    public class AccountsService : IAccountsService
    {

        public AccountsService()
        {
            
        }

        public async Task<PaginatedResult<AccountDTO>> GetAllAccountsAsync(int? pageNumber, int? pageSize)
        {
            throw new NotImplementedException();
        }

        public async Task<PaginatedResult<AccountDTO>> SearchAccountsAsync(AccountDTO accountCriteria, int? pageNumber, int? pageSize)
        {
            throw new NotImplementedException();
        }

        public async Task<AccountDTO?> GetAccountByIdAsync(Guid accountId)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult> AddAccountAsync(AccountDTO newAccount)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult> UpdateAccountAsync(AccountDTO updatedAccount)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult> DeleteAccountAsync(Guid accountId)
        {
            throw new NotImplementedException();
        }
    }
}
