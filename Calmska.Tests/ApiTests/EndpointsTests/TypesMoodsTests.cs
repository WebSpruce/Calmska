using Calmska.Api.Endpoints;
using Calmska.Application.DTO;
using Calmska.Domain.Common;
using Calmska.Infrastructure.Persistence;
using Calmska.Infrastructure.Persistence.Models;
using Calmska.Tests.ApiTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Calmska.Tests.ApiTests.EndpointsTests
{
    public class TypesMoodsTests : IClassFixture<CalmskaApiFactory>
    {
        private readonly CalmskaApiFactory _factory;
        private readonly HttpClient _client;
        public TypesMoodsTests(CalmskaApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllTypesMoods_ShouldReturnOk_WhenTypesExist()
        {
            int id = 100;
            await SeedDocumentAsync(id, type:"Excited");
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/types_moods?pageNumber=1&pageSize=10";
            
            using var response = await _client.GetAsync(endpoint);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<PaginatedResult<Types_MoodDTO>>();
            content.Should().NotBeNull();
            content.TotalCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetAllTypesMoods_ShouldReturnNotFound_WhenNoTypesExist()
        {
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/types_moods?pageNumber=0&pageSize=10";
            
            using var response = await _client.GetAsync(endpoint);
            
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var errorMessage = await response.Content.ReadAsStringAsync();
            errorMessage.Should().Contain("Types not found");
        }

        [Fact]
        public async Task SearchListTypesMoods_ShouldReturnOk_WhenTypesMatchCriteria()
        {
            int id = 100;
            await SeedDocumentAsync(id, type:"Excited");
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/types_moods/searchList?Type=Excited&pageNumber=1&pageSize=5";
            
            using var response = await _client.GetAsync(endpoint);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<PaginatedResult<Types_MoodDTO>>();
            content.Should().NotBeNull();
            content.TotalCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task SearchListTypesMoods_ShouldReturnNotFound_WhenNoTypesMatchCriteria()
        {
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/types_moods/searchList?Type=nonexistent&pageNumber=1&pageSize=5";
            
            using var response = await _client.GetAsync(endpoint);
            
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var errorMessage = await response.Content.ReadAsStringAsync();
            errorMessage.Should().Contain("Types not found");
        }

        [Fact]
        public async Task SearchTypeMood_ShouldReturnOk_WhenTypeExists()
        {
            int id = 100;
            await SeedDocumentAsync(id);
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/types_moods/search?TypeId={id}";
            
            using var response = await _client.GetAsync(endpoint);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<Types_MoodDTO>();
            content.Should().NotBeNull();
            content.TypeId.Should().Be(id);
        }

        [Fact]
        public async Task SearchTypeMood_ShouldReturnNotFound_WhenTypeDoesNotExist()
        {
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/types_moods/search?TypeId=9999";
            
            using var response = await _client.GetAsync(endpoint);
            
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var errorMessage = await response.Content.ReadAsStringAsync();
            errorMessage.Should().Contain("Types not found");
        }

        [Fact]
        public async Task AddTypeMood_ShouldReturnCreated_WhenTypeIsValid()
        {
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/types_moods";
            var typeMood = new Types_MoodDTO
            {
                TypeId = 100, 
                Type = "Excited"
            };
            
            using var response = await _client.PostAsJsonAsync(endpoint, typeMood);
            
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var saved = await QuerySender.QueryDbAsync(_factory,db => db.Types_MoodDb
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.TypeId == 100));

            saved.Should().NotBeNull();
            saved.Type.Should().Be("Excited");
        }

        [Fact]
        public async Task UpdateTypeMood_ShouldReturnOk_WhenTypeIsValid()
        {
            int id = 100;
            await SeedDocumentAsync(id);
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/types_moods";
            var typeMood = new Types_MoodDTO
            {
                TypeId = id, 
                Type = "Calm"
            };
            
            using var response = await _client.PutAsJsonAsync(endpoint, typeMood);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var successMessage = await response.Content.ReadAsStringAsync();
            successMessage.Should().Contain("Type updated successfully");
            
            var updated = await QuerySender.QueryDbAsync(_factory,db => db.Types_MoodDb
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.TypeId == id));
            
            updated.Should().NotBeNull();
            updated.Type.Should().Be("Calm");
        }

        [Fact]
        public async Task UpdateTypeMood_ShouldReturnBadRequest_WhenTypeIsInvalid()
        {
            int id = 100;
            await SeedDocumentAsync(id);
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/types_moods";
            var typeMood = new Types_MoodDTO
            {
                TypeId = 0, 
                Type = ""
            };
            using var response = await _client.PutAsJsonAsync(endpoint, typeMood);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteTypeMood_ShouldReturnOk_WhenTypeExists()
        {
            int id = 100;
            await SeedDocumentAsync(id);
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/types_moods/{id}";
  
            using var response = await _client.DeleteAsync(endpoint);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var successMessage = await response.Content.ReadAsStringAsync();
            successMessage.Should().Contain("Type deleted successfully");
        }

        [Fact]
        public async Task DeleteTypeMood_ShouldReturnBadRequest_WhenTypeDoesNotExist()
        {
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/types_moods/9999";

            using var response = await _client.DeleteAsync(endpoint);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        private async Task<Types_MoodDocument> SeedDocumentAsync(
            int typeId = 1,
            string type = "test")
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CalmskaDbContext>();

            var tm = new Types_MoodDocument()
            {
                TypeId = typeId,
                Type = type
            };

            await db.Types_MoodDb.AddAsync(tm);
            await db.SaveChangesAsync();

            return tm;
        }
    }
}
