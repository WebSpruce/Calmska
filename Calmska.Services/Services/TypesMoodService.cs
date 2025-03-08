using Calmska.Models.DTO;
using Calmska.Models.Models;
using Calmska.Services.Helper;
using Calmska.Services.Interfaces;

namespace Calmska.Services.Services
{
    public class TypesMoodService : ITypesService<Types_MoodDTO>
    {
        private readonly HttpClient _httpClient;
        public TypesMoodService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OperationResultT<PaginatedResult<Types_MoodDTO?>>> GetAllAsync(int? pageNumber, int? pageSize)
        {
            return await HttpClientHelper.GetAsync<PaginatedResult<Types_MoodDTO?>>(_httpClient, "/types_moods");
        }

        public async Task<OperationResultT<PaginatedResult<IEnumerable<Types_MoodDTO?>>>> SearchAllByArgumentAsync(Types_MoodDTO typesCriteria, int? pageNumber, int? pageSize)
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
                endpoint = $"/types_moods/searchList?{queryString}";
            }

            return await HttpClientHelper.GetAsync<PaginatedResult<IEnumerable<Types_MoodDTO?>>>(_httpClient, endpoint);
        }

        public async Task<OperationResultT<Types_MoodDTO?>> GetByArgumentAsync(Types_MoodDTO typesCriteria)
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
                endpoint = $"/types_moods/search?{queryString}";
            }

            return await HttpClientHelper.GetAsync<Types_MoodDTO?>(_httpClient, endpoint);
        }

        public async Task<OperationResultT<bool>> AddAsync(Types_MoodDTO newTip)
        {
            return await HttpClientHelper.PostAsync<Types_MoodDTO?>(_httpClient, "/types_moods", newTip);
        }

        public async Task<OperationResultT<bool>> UpdateAsync(Types_MoodDTO updatedTip)
        {
            return await HttpClientHelper.PutAsync<Types_MoodDTO?>(_httpClient, "/types_moods", updatedTip);
        }

        public async Task<OperationResultT<bool>> DeleteAsync(int TypeId)
        {
            return await HttpClientHelper.DeleteAsync(_httpClient, $"/types_moods?TypeId={TypeId}");
        }
    }
}
