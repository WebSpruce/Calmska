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
    public class SettingsService : IService<SettingsDTO>
    {
        private readonly HttpClient _httpClient;
        public SettingsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OperationResultT<PaginatedResult<SettingsDTO?>>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            return await HttpClientHelper.GetAsync<PaginatedResult<SettingsDTO?>>(_httpClient, "settings", token);
        }

        public async Task<OperationResultT<PaginatedResult<SettingsDTO?>>> SearchAllByArgumentAsync(SettingsDTO settingsCriteria, int? pageNumber, int? pageSize, CancellationToken token)
        {
            List<string> endpointParameters = new();
            string endpoint = string.Empty;

            if (settingsCriteria != null)
            {
                if (settingsCriteria.SettingsId.HasValue)
                {
                    endpointParameters.Add($"SettingsId={settingsCriteria.SettingsId}");
                }
                if (!string.IsNullOrEmpty(settingsCriteria.Color))
                {
                    endpointParameters.Add($"Color={Uri.EscapeDataString(settingsCriteria.Color)}");
                }
                if (settingsCriteria.PomodoroBreak != null)
                {
                    endpointParameters.Add($"PomodoroBreak={settingsCriteria.PomodoroBreak}");
                }
                if (settingsCriteria.PomodoroTimer != null)
                {
                    endpointParameters.Add($"PomodoroTimer={settingsCriteria.PomodoroTimer}");
                }
                if (settingsCriteria.UserId.HasValue)
                {
                    endpointParameters.Add($"UserId={settingsCriteria.UserId}");
                }
                var queryString = string.Join("&", endpointParameters);
                endpoint = $"settings/searchList?{queryString}";
            }

            return await HttpClientHelper.GetAsync<PaginatedResult<SettingsDTO?>>(_httpClient, endpoint, token);
        }

        public async Task<OperationResultT<SettingsDTO?>> GetByArgumentAsync(SettingsDTO settingsCriteria, CancellationToken token)
        {
            List<string> endpointParameters = new();
            string endpoint = string.Empty;

            if (settingsCriteria != null)
            {
                if (settingsCriteria.SettingsId.HasValue)
                {
                    endpointParameters.Add($"SettingsId={settingsCriteria.SettingsId}");
                }
                if (!string.IsNullOrEmpty(settingsCriteria.Color))
                {
                    endpointParameters.Add($"Color={Uri.EscapeDataString(settingsCriteria.Color)}");
                }
                if (settingsCriteria.PomodoroBreak != null)
                {
                    endpointParameters.Add($"PomodoroBreak={settingsCriteria.PomodoroBreak}");
                }
                if (settingsCriteria.PomodoroTimer != null)
                {
                    endpointParameters.Add($"PomodoroTimer={settingsCriteria.PomodoroTimer}");
                }
                if (settingsCriteria.UserId.HasValue)
                {
                    endpointParameters.Add($"UserId={settingsCriteria.UserId}");
                }
                var queryString = string.Join("&", endpointParameters);
                endpoint = $"settings/search?{queryString}";
            }

            return await HttpClientHelper.GetAsync<SettingsDTO?>(_httpClient, endpoint, token);
        }

        public async Task<OperationResultT<bool>> AddAsync(SettingsDTO newSettings, CancellationToken token)
        {
            return await HttpClientHelper.PostAsync<SettingsDTO?>(_httpClient, "settings", newSettings, token);
        }

        public async Task<OperationResultT<bool>> UpdateAsync(SettingsDTO updatedSettings, CancellationToken token)
        {
            return await HttpClientHelper.PutAsync<SettingsDTO?>(_httpClient, "settings", updatedSettings, token);
        }

        public async Task<OperationResultT<bool>> DeleteAsync(Guid settingsId, CancellationToken token)
        {
            return await HttpClientHelper.DeleteAsync(_httpClient, $"settings?settingsId={settingsId}", token);
        }
    }
}
