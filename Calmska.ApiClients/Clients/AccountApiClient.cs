using Calmska.ApiClients.Interfaces;
using Calmska.Application.DTO;
using Calmska.Domain.Common;

namespace Calmska.ApiClients.Clients
{
    public class AccountApiClient : IAccountApiClient
    {
        private readonly IHttpClientService _httpClientService;
        public AccountApiClient(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<OperationResultT<PaginatedResult<AccountDTO?>>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            var endpoint = $"accounts?pageNumber={pageNumber ?? 1}&pageSize={pageSize ?? 10}";
            return await _httpClientService.GetAsync<PaginatedResult<AccountDTO?>>(endpoint, token);
        }

        public async Task<OperationResultT<PaginatedResult<AccountDTO?>>> SearchAllByArgumentAsync(AccountDTO accountCriteria, int? pageNumber, int? pageSize, CancellationToken token)
        {
            var query = BuildAccountQueryString(accountCriteria);
            var endpoint = string.IsNullOrEmpty(query) 
                ? "accounts/searchList" 
                : $"accounts/searchList?{query}";

            return await _httpClientService.GetAsync<PaginatedResult<AccountDTO?>>(endpoint, token);
        }

        public async Task<OperationResultT<AccountDTO?>> GetByArgumentAsync(AccountDTO accountCriteria, CancellationToken token)
        {
            var queryString = BuildAccountQueryString(accountCriteria);
            var endpoint = string.IsNullOrEmpty(queryString) 
                ? "accounts/search" 
                : $"accounts/search?{queryString}";

            return await _httpClientService.GetAsync<AccountDTO?>(endpoint, token);
        }

        public async Task<OperationResultT<bool>> LoginAsync(LoginDTO loginDto, CancellationToken token)
        {
            return await _httpClientService.PostAsync<LoginDTO>("accounts/login", loginDto, token);
        }

        public async Task<OperationResultT<bool>> AddAsync(AccountDTO newAccount, CancellationToken token)
        {
            return await _httpClientService.PostAsync<AccountDTO?>("accounts", newAccount, token);
        }

        public async Task<OperationResultT<bool>> UpdateAsync(AccountDTO updatedAccount, CancellationToken token)
        {
            return await _httpClientService.PutAsync<AccountDTO?>("accounts", updatedAccount, token);
        }

        public async Task<OperationResultT<bool>> DeleteAsync(Guid accountId, CancellationToken token)
        {
            return await _httpClientService.DeleteAsync($"accounts?accountId={accountId}", token);
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
