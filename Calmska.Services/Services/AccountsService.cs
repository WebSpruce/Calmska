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

        public async Task<OperationResultT<IEnumerable<AccountDTO?>>> GetAllAccountsAsync(int? pageNumber, int? pageSize)
        {
            return await HttpClientHelper.GetAsync<IEnumerable<AccountDTO?>>(_httpClient, "/accounts");
        }

        public async Task<OperationResultT<PaginatedResult<IEnumerable<AccountDTO?>>>> SearchAccountsByArgumentAsync(AccountDTO accountCriteria, int? pageNumber, int? pageSize)
        {
            return await HttpClientHelper.GetAsync<PaginatedResult<IEnumerable<AccountDTO?>>>(_httpClient, "/accounts/searchList");
        }

        public async Task<OperationResultT<AccountDTO?>> GetAccountByArgumentAsync(AccountDTO accountCriteria)
        {
            return await HttpClientHelper.GetAsync<AccountDTO?>(_httpClient, "/accounts/search");
        }

        public async Task<OperationResultT<bool>> AddAccountAsync(AccountDTO newAccount)
        {
            return await HttpClientHelper.PostAsync<AccountDTO?>(_httpClient, "/accounts", newAccount);
        }

        public async Task<OperationResultT<bool>> UpdateAccountAsync(AccountDTO updatedAccount)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResultT<bool>> DeleteAccountAsync(Guid accountId)
        {
            throw new NotImplementedException();
        }
    }
}
