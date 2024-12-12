using Calmska.Models.DTO;
using Calmska.Models.Models;
using Calmska.Services.Helper;
using Calmska.Services.Interfaces;

namespace Calmska.Services.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly HttpClient _httpClient;
        public AccountsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OperationResultT<IEnumerable<AccountDTO>>> GetAllAccountsAsync(int? pageNumber, int? pageSize)
        {
            return await HttpClientHelper.GetAsync<IEnumerable<AccountDTO>>(_httpClient, "/accounts");
        }

        public async Task<PaginatedResult<AccountDTO>> SearchAccountsByArgumentAsync(AccountDTO accountCriteria, int? pageNumber, int? pageSize)
        {
            throw new NotImplementedException();
        }

        public async Task<AccountDTO?> GetAccountByArgumentAsync(Guid accountId)
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
