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
    public class TypesMoodService : ITypesService<Types_MoodDTO>
    {
        private readonly HttpClient _httpClient;
        public TypesMoodService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OperationResultT<PaginatedResult<Types_MoodDTO?>>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            return await HttpClientHelper.GetAsync<PaginatedResult<Types_MoodDTO?>>(_httpClient, "types_moods", token);
        }

        public async Task<OperationResultT<PaginatedResult<IEnumerable<Types_MoodDTO?>>>> SearchAllByArgumentAsync(Types_MoodDTO typesCriteria, int? pageNumber, int? pageSize, CancellationToken token)
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
                endpoint = $"types_moods/searchList?{queryString}";
            }

            return await HttpClientHelper.GetAsync<PaginatedResult<IEnumerable<Types_MoodDTO?>>>(_httpClient, endpoint, token);
        }

        public async Task<OperationResultT<Types_MoodDTO?>> GetByArgumentAsync(Types_MoodDTO typesCriteria, CancellationToken token)
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
                endpoint = $"types_moods/search?{queryString}";
            }

            return await HttpClientHelper.GetAsync<Types_MoodDTO?>(_httpClient, endpoint, token);
        }

        public async Task<OperationResultT<bool>> AddAsync(Types_MoodDTO newTip, CancellationToken token)
        {
            return await HttpClientHelper.PostAsync<Types_MoodDTO?>(_httpClient, "types_moods", newTip, token);
        }

        public async Task<OperationResultT<bool>> UpdateAsync(Types_MoodDTO updatedTip, CancellationToken token)
        {
            return await HttpClientHelper.PutAsync<Types_MoodDTO?>(_httpClient, "types_moods", updatedTip, token);
        }

        public async Task<OperationResultT<bool>> DeleteAsync(int TypeId, CancellationToken token)
        {
            return await HttpClientHelper.DeleteAsync(_httpClient, $"types_moods?TypeId={TypeId}", token);
        }
    }
}
