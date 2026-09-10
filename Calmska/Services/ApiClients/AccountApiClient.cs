using Calmska.Application.DTO;
using Calmska.Domain.Common;
using Calmska.Helper;
using Calmska.Interfaces;

namespace Calmska.Services.ApiClients
{
    public class AccountApiClient : IAccountApiClient
    {
        private readonly HttpClient _httpClient;
        public AccountApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OperationResultT<PaginatedResult<AccountDTO?>>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            var endpoint = $"accounts?pageNumber={pageNumber ?? 1}&pageSize={pageSize ?? 10}";
            return await HttpClientHelper.GetAsync<PaginatedResult<AccountDTO?>>(_httpClient, endpoint, token);
        }

        public async Task<OperationResultT<PaginatedResult<AccountDTO?>>> SearchAllByArgumentAsync(AccountDTO accountCriteria, int? pageNumber, int? pageSize, CancellationToken token)
        {
            var query = BuildAccountQueryString(accountCriteria);
            var endpoint = string.IsNullOrEmpty(query) 
                ? "accounts/searchList" 
                : $"accounts/searchList?{query}";

            return await HttpClientHelper.GetAsync<PaginatedResult<AccountDTO?>>(_httpClient, endpoint, token);
        }

        public async Task<OperationResultT<AccountDTO?>> GetByArgumentAsync(AccountDTO accountCriteria, CancellationToken token)
        {
            var queryString = BuildAccountQueryString(accountCriteria);
            var endpoint = string.IsNullOrEmpty(queryString) 
                ? "accounts/search" 
                : $"accounts/search?{queryString}";

            return await HttpClientHelper.GetAsync<AccountDTO?>(_httpClient, endpoint, token);
        }

        public async Task<OperationResultT<bool>> LoginAsync(AccountDTO accountCriteria, CancellationToken token)
        {
            var queryString = BuildAccountQueryString(accountCriteria);
            var endpoint = string.IsNullOrEmpty(queryString) 
                ? "accounts/login" 
                : $"accounts/login?{queryString}";

            return await HttpClientHelper.GetAsync<bool>(_httpClient, endpoint, token);
        }

        public async Task<OperationResultT<bool>> AddAsync(AccountDTO newAccount, CancellationToken token)
        {
            return await HttpClientHelper.PostAsync<AccountDTO?>(_httpClient, "accounts", newAccount, token);
        }

        public async Task<OperationResultT<bool>> UpdateAsync(AccountDTO updatedAccount, CancellationToken token)
        {
            return await HttpClientHelper.PutAsync<AccountDTO?>(_httpClient, "accounts", updatedAccount, token);
        }

        public async Task<OperationResultT<bool>> DeleteAsync(Guid accountId, CancellationToken token)
        {
            return await HttpClientHelper.DeleteAsync(_httpClient, $"accounts?accountId={accountId}", token);
        }
        
        private static string BuildAccountQueryString(AccountDTO criteria)
        {
            if (criteria is null) return string.Empty;

            var parameters = new List<string>();

            if (criteria.UserId.HasValue) 
                parameters.Add($"userId={criteria.UserId}");
            if (!string.IsNullOrEmpty(criteria.UserName)) 
                parameters.Add($"userName={Uri.EscapeDataString(criteria.UserName)}");
            if (!string.IsNullOrEmpty(criteria.Email)) 
                parameters.Add($"email={Uri.EscapeDataString(criteria.Email)}");
            if (!string.IsNullOrEmpty(criteria.PasswordHashed))
                parameters.Add($"passwordHashed={Uri.EscapeDataString(criteria.PasswordHashed)}");
            
            return string.Join("&", parameters);
        }
    }
}
