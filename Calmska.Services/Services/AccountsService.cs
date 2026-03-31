using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Calmska.Application.DTO;
using Calmska.Domain.Common;
using Calmska.Services.Helper;
using Calmska.Services.Interfaces;

namespace Calmska.Services.Services
{
    public class AccountsService : IAccountService
    {
        private readonly HttpClient _httpClient;
        public AccountsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OperationResultT<PaginatedResult<AccountDTO?>>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            return await HttpClientHelper.GetAsync<PaginatedResult<AccountDTO?>>(_httpClient, "accounts", token);
        }

        public async Task<OperationResultT<PaginatedResult<AccountDTO?>>> SearchAllByArgumentAsync(AccountDTO accountCriteria, int? pageNumber, int? pageSize, CancellationToken token)
        {
            List<string> endpointParameters = new();
            string endpoint = string.Empty;

            if (accountCriteria != null)
            {
                if (accountCriteria.UserId.HasValue)
                {
                    endpointParameters.Add($"UserId={accountCriteria.UserId}");
                }
                if (!string.IsNullOrEmpty(accountCriteria.UserName))
                {
                    endpointParameters.Add($"UserName={Uri.EscapeDataString(accountCriteria.UserName)}");
                }
                if (!string.IsNullOrEmpty(accountCriteria.Email))
                {
                    endpointParameters.Add($"Email={Uri.EscapeDataString(accountCriteria.Email)}");
                }
                if (!string.IsNullOrEmpty(accountCriteria.PasswordHashed))
                {
                    endpointParameters.Add($"PasswordHashed={Uri.EscapeDataString(accountCriteria.PasswordHashed)}");
                }
                var queryString = string.Join("&", endpointParameters);
                endpoint = $"accounts/searchList?{queryString}";
            }

            return await HttpClientHelper.GetAsync<PaginatedResult<AccountDTO?>>(_httpClient, endpoint, token);
        }

        public async Task<OperationResultT<AccountDTO?>> GetByArgumentAsync(AccountDTO accountCriteria, CancellationToken token)
        {
            List<string> endpointParameters = new();
            string endpoint = string.Empty;

            if (accountCriteria != null)
            {
                if (accountCriteria.UserId.HasValue)
                {
                    endpointParameters.Add($"UserId={accountCriteria.UserId}");
                }
                if (!string.IsNullOrEmpty(accountCriteria.UserName))
                {
                    endpointParameters.Add($"UserName={Uri.EscapeDataString(accountCriteria.UserName)}");
                }
                if (!string.IsNullOrEmpty(accountCriteria.Email))
                {
                    endpointParameters.Add($"Email={Uri.EscapeDataString(accountCriteria.Email)}");
                }
                if (!string.IsNullOrEmpty(accountCriteria.PasswordHashed))
                {
                    endpointParameters.Add($"PasswordHashed={Uri.EscapeDataString(accountCriteria.PasswordHashed)}");
                }
                var queryString = string.Join("&", endpointParameters);
                endpoint = $"accounts/search?{queryString}";
            }

            return await HttpClientHelper.GetAsync<AccountDTO?>(_httpClient, endpoint, token);
        }

        public async Task<OperationResultT<bool>> LoginAsync(AccountDTO accountCriteria, CancellationToken token)
        {
            List<string> endpointParameters = new();
            string endpoint = string.Empty;

            if (accountCriteria != null)
            {
                if (!string.IsNullOrEmpty(accountCriteria.Email))
                {
                    endpointParameters.Add($"email={Uri.EscapeDataString(accountCriteria.Email)}");
                }
                if (!string.IsNullOrEmpty(accountCriteria.PasswordHashed))
                {
                    endpointParameters.Add($"password={Uri.EscapeDataString(accountCriteria.PasswordHashed)}");
                }
                var queryString = string.Join("&", endpointParameters);
                endpoint = $"accounts/login?{queryString}";
            }

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
    }
}
