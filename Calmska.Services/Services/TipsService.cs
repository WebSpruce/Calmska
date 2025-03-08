using Calmska.Models.DTO;
using Calmska.Models.Models;
using Calmska.Services.Helper;
using Calmska.Services.Interfaces;

namespace Calmska.Services.Services
{
    public class TipsService : IService<TipsDTO>
    {
        private readonly HttpClient _httpClient;
        public TipsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OperationResultT<PaginatedResult<TipsDTO?>>> GetAllAsync(int? pageNumber, int? pageSize)
        {
            return await HttpClientHelper.GetAsync<PaginatedResult<TipsDTO?>>(_httpClient, "/tips");
        }

        public async Task<OperationResultT<PaginatedResult<IEnumerable<TipsDTO?>>>> SearchAllByArgumentAsync(TipsDTO tipsCriteria, int? pageNumber, int? pageSize)
        {
            List<string> endpointParameters = new();
            string endpoint = string.Empty;

            if (tipsCriteria != null)
            {
                if (tipsCriteria.TipId.HasValue)
                {
                    endpointParameters.Add($"TipId={tipsCriteria.TipId}");
                }
                if (!string.IsNullOrEmpty(tipsCriteria.Content))
                {
                    endpointParameters.Add($"Content={Uri.EscapeDataString(tipsCriteria.Content)}");
                }
                if (tipsCriteria.TipsTypeId != null)
                {
                    endpointParameters.Add($"Type={tipsCriteria.TipsTypeId}");
                }
                var queryString = string.Join("&", endpointParameters);
                endpoint = $"/tips/searchList?{queryString}";
            }

            return await HttpClientHelper.GetAsync<PaginatedResult<IEnumerable<TipsDTO?>>>(_httpClient, endpoint);
        }

        public async Task<OperationResultT<TipsDTO?>> GetByArgumentAsync(TipsDTO tipsCriteria)
        {
            List<string> endpointParameters = new();
            string endpoint = string.Empty;

            if (tipsCriteria != null)
            {
                if (tipsCriteria.TipId.HasValue)
                {
                    endpointParameters.Add($"TipId={tipsCriteria.TipId}");
                }
                if (!string.IsNullOrEmpty(tipsCriteria.Content))
                {
                    endpointParameters.Add($"Content={Uri.EscapeDataString(tipsCriteria.Content)}");
                }
                if (tipsCriteria.TipsTypeId != null)
                {
                    endpointParameters.Add($"Type={tipsCriteria.TipsTypeId}");
                }
                var queryString = string.Join("&", endpointParameters);
                endpoint = $"/tips/search?{queryString}";
            }

            return await HttpClientHelper.GetAsync<TipsDTO?>(_httpClient, endpoint);
        }

        public async Task<OperationResultT<bool>> AddAsync(TipsDTO newTip)
        {
            return await HttpClientHelper.PostAsync<TipsDTO?>(_httpClient, "/tips", newTip);
        }

        public async Task<OperationResultT<bool>> UpdateAsync(TipsDTO updatedTip)
        {
            return await HttpClientHelper.PutAsync<TipsDTO?>(_httpClient, "/tips", updatedTip);
        }

        public async Task<OperationResultT<bool>> DeleteAsync(Guid tipId)
        {
            return await HttpClientHelper.DeleteAsync(_httpClient, $"/tips?tipId={tipId}");
        }
    }
}
