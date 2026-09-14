using Calmska.ApiClients.Interfaces;
using Calmska.Application.DTO;
using Calmska.Domain.Common;

namespace Calmska.ApiClients.Clients
{
    public class SettingsApiClient : IApiClient<SettingsDTO>
    {
        private readonly IHttpClientService _httpClientService;
        public SettingsApiClient(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<OperationResultT<PaginatedResult<SettingsDTO?>>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            return await _httpClientService.GetAsync<PaginatedResult<SettingsDTO?>>("settings", token);
        }

        public async Task<OperationResultT<PaginatedResult<SettingsDTO?>>> SearchAllByArgumentAsync(SettingsDTO settingsCriteria, int? pageNumber, int? pageSize, CancellationToken token)
        {
            var query = BuildQueryString(settingsCriteria);
            string endpoint = $"settings/searchList?{query}";

            return await _httpClientService.GetAsync<PaginatedResult<SettingsDTO?>>(endpoint, token);
        }

        public async Task<OperationResultT<SettingsDTO?>> GetByArgumentAsync(SettingsDTO settingsCriteria, CancellationToken token)
        {
            var query = BuildQueryString(settingsCriteria);
            string endpoint = $"settings/search?{query}";

            return await _httpClientService.GetAsync<SettingsDTO?>(endpoint, token);
        }

        public async Task<OperationResultT<bool>> AddAsync(SettingsDTO newSettings, CancellationToken token)
        {
            return await _httpClientService.PostAsync<SettingsDTO?>("settings", newSettings, token);
        }

        public async Task<OperationResultT<bool>> UpdateAsync(SettingsDTO updatedSettings, CancellationToken token)
        {
            return await _httpClientService.PutAsync<SettingsDTO?>("settings", updatedSettings, token);
        }

        public async Task<OperationResultT<bool>> DeleteAsync(Guid settingsId, CancellationToken token)
        {
            return await _httpClientService.DeleteAsync($"settings?settingsId={settingsId}", token);
        }
        
        private static string BuildQueryString(SettingsDTO criteria)
        {
            if (criteria is null) return string.Empty;

            var parameters = new List<string>();

            if (criteria.SettingsId.HasValue)
                parameters.Add($"SettingsId={criteria.SettingsId}");
            if (!string.IsNullOrEmpty(criteria.Color))
                parameters.Add($"Color={Uri.EscapeDataString(criteria.Color)}");
            if (criteria.PomodoroBreak != null)
                parameters.Add($"PomodoroBreak={criteria.PomodoroBreak}");
            if (criteria.PomodoroTimer != null)
                parameters.Add($"PomodoroTimer={criteria.PomodoroTimer}");
            if (criteria.UserId.HasValue)
                parameters.Add($"UserId={criteria.UserId}");
            
            return string.Join("&", parameters);
        }
    }
}
