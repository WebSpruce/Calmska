﻿using Calmska.Models.DTO;
using Calmska.Models.Models;
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

        public async Task<OperationResultT<PaginatedResult<AccountDTO?>>> GetAllAsync(int? pageNumber, int? pageSize)
        {
            return await HttpClientHelper.GetAsync<PaginatedResult<AccountDTO?>>(_httpClient, "/accounts");
        }

        public async Task<OperationResultT<PaginatedResult<AccountDTO?>>> SearchAllByArgumentAsync(AccountDTO accountCriteria, int? pageNumber, int? pageSize)
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
                endpoint = $"/accounts/searchList?{queryString}";
            }

            return await HttpClientHelper.GetAsync<PaginatedResult<AccountDTO?>>(_httpClient, endpoint);
        }

        public async Task<OperationResultT<AccountDTO?>> GetByArgumentAsync(AccountDTO accountCriteria)
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
                endpoint = $"/accounts/search?{queryString}";
            }

            return await HttpClientHelper.GetAsync<AccountDTO?>(_httpClient, endpoint);
        }

        public async Task<OperationResultT<bool>> LoginAsync(AccountDTO accountCriteria)
        {
            List<string> endpointParameters = new();
            string endpoint = string.Empty;

            if (accountCriteria != null)
            {
                if (!string.IsNullOrEmpty(accountCriteria.Email))
                {
                    endpointParameters.Add($"Email={Uri.EscapeDataString(accountCriteria.Email)}");
                }
                if (!string.IsNullOrEmpty(accountCriteria.PasswordHashed))
                {
                    endpointParameters.Add($"PasswordHashed={Uri.EscapeDataString(accountCriteria.PasswordHashed)}");
                }
                var queryString = string.Join("&", endpointParameters);
                endpoint = $"/accounts/login?{queryString}";
            }

            return await HttpClientHelper.GetAsync<bool>(_httpClient, endpoint);
        }

        public async Task<OperationResultT<bool>> AddAsync(AccountDTO newAccount)
        {
            return await HttpClientHelper.PostAsync<AccountDTO?>(_httpClient, "/accounts", newAccount);
        }

        public async Task<OperationResultT<bool>> UpdateAsync(AccountDTO updatedAccount)
        {
            return await HttpClientHelper.PutAsync<AccountDTO?>(_httpClient, "/accounts", updatedAccount);
        }

        public async Task<OperationResultT<bool>> DeleteAsync(Guid accountId)
        {
            return await HttpClientHelper.DeleteAsync(_httpClient, $"/accounts?accountId={accountId}");
        }
    }
}
