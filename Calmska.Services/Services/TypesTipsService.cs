using Calmska.Models.DTO;
using Calmska.Models.Models;
using Calmska.Services.Helper;
using Calmska.Services.Interfaces;

namespace Calmska.Services.Services
{
    public class TypesTipsService : ITypesService<Types_TipsDTO>
    {
        private readonly HttpClient _httpClient;
        public TypesTipsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OperationResultT<PaginatedResult<Types_TipsDTO?>>> GetAllAsync(int? pageNumber, int? pageSize)
        {
            return await HttpClientHelper.GetAsync<PaginatedResult<Types_TipsDTO?>>(_httpClient, "/types_tips");
        }

        public async Task<OperationResultT<PaginatedResult<IEnumerable<Types_TipsDTO?>>>> SearchAllByArgumentAsync(Types_TipsDTO typesCriteria, int? pageNumber, int? pageSize)
        {
            List<string> endpointParameters = new();
            string endpoint = string.Empty;

            if (typesCriteria != null)
            {
                if (typesCriteria.TypeId.HasValue)
                {
                    endpointParameters.Add($"TypeId={typesCriteria.TypeId}");
                }
                if (!string.IsNullOrEmpty(typesCriteria.Type))
                {
                    endpointParameters.Add($"Type={Uri.EscapeDataString(typesCriteria.Type)}");
                }
                var queryString = string.Join("&", endpointParameters);
                endpoint = $"/types_tips/searchList?{queryString}";
            }

            return await HttpClientHelper.GetAsync<PaginatedResult<IEnumerable<Types_TipsDTO?>>>(_httpClient, endpoint);
        }

        public async Task<OperationResultT<Types_TipsDTO?>> GetByArgumentAsync(Types_TipsDTO typesCriteria)
        {
            List<string> endpointParameters = new();
            string endpoint = string.Empty;

            if (typesCriteria != null)
            {
                if (typesCriteria.TypeId.HasValue)
                {
                    endpointParameters.Add($"TypeId={typesCriteria.TypeId}");
                }
                if (!string.IsNullOrEmpty(typesCriteria.Type))
                {
                    endpointParameters.Add($"Type={Uri.EscapeDataString(typesCriteria.Type)}");
                }
                var queryString = string.Join("&", endpointParameters);
                endpoint = $"/types_tips/search?{queryString}";
            }

            return await HttpClientHelper.GetAsync<Types_TipsDTO?>(_httpClient, endpoint);
        }

        public async Task<OperationResultT<bool>> AddAsync(Types_TipsDTO newTip)
        {
            return await HttpClientHelper.PostAsync<Types_TipsDTO?>(_httpClient, "/types_tips", newTip);
        }

        public async Task<OperationResultT<bool>> UpdateAsync(Types_TipsDTO updatedTip)
        {
            return await HttpClientHelper.PutAsync<Types_TipsDTO?>(_httpClient, "/types_tips", updatedTip);
        }

        public async Task<OperationResultT<bool>> DeleteAsync(int TypeId)
        {
            return await HttpClientHelper.DeleteAsync(_httpClient, $"/types_tips?TypeId={TypeId}");
        }
    }
}
