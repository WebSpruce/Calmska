using Calmska.Models.DTO;
using Calmska.Models.Models;
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

        public async Task<OperationResultT<PaginatedResult<SettingsDTO?>>> GetAllAsync(int? pageNumber, int? pageSize)
        {
            return await HttpClientHelper.GetAsync<PaginatedResult<SettingsDTO?>>(_httpClient, "/settings");
        }

        public async Task<OperationResultT<PaginatedResult<IEnumerable<SettingsDTO?>>>> SearchAllByArgumentAsync(SettingsDTO settingsCriteria, int? pageNumber, int? pageSize)
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
                endpoint = $"/settings/searchList?{queryString}";
            }

            return await HttpClientHelper.GetAsync<PaginatedResult<IEnumerable<SettingsDTO?>>>(_httpClient, endpoint);
        }

        public async Task<OperationResultT<SettingsDTO?>> GetByArgumentAsync(SettingsDTO settingsCriteria)
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
                endpoint = $"/settings/search?{queryString}";
            }

            return await HttpClientHelper.GetAsync<SettingsDTO?>(_httpClient, endpoint);
        }

        public async Task<OperationResultT<bool>> AddAsync(SettingsDTO newSettings)
        {
            return await HttpClientHelper.PostAsync<SettingsDTO?>(_httpClient, "/settings", newSettings);
        }

        public async Task<OperationResultT<bool>> UpdateAsync(SettingsDTO updatedSettings)
        {
            return await HttpClientHelper.PutAsync<SettingsDTO?>(_httpClient, "/settings", updatedSettings);
        }

        public async Task<OperationResultT<bool>> DeleteAsync(Guid settingsId)
        {
            return await HttpClientHelper.DeleteAsync(_httpClient, $"/settings?settingsId={settingsId}");
        }
    }
}
