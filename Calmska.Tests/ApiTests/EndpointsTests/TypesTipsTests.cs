using Calmska.Api.Endpoints;
using Calmska.Application.DTO;
using Calmska.Domain.Common;
using Calmska.Infrastructure.Persistence;
using Calmska.Infrastructure.Persistence.Models;
using Calmska.Tests.ApiTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Calmska.Tests.ApiTests.EndpointsTests
{
    public class TypesTipsTests : IClassFixture<CalmskaApiFactory>
    {
        private readonly CalmskaApiFactory _factory;
        private readonly HttpClient _client;
        public TypesTipsTests(CalmskaApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllTypesTips_ShouldReturnOk_WhenTypesExist()
        {
            var id = 100;
            await SeedDocumentAsync(id, type:"Health");
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/types_tips?pageNumber=1&pageSize=10";
            
            using var response = await _client.GetAsync(endpoint);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<PaginatedResult<Types_TipsDTO>>();
            content.Should().NotBeNull();
            content.TotalCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetAllTypesTips_ShouldReturnNotFound_WhenNoTypesExist()
        {
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/types_tips?pageNumber=0&pageSize=10";
            
            using var response = await _client.GetAsync(endpoint);
            
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var errorMessage = await response.Content.ReadAsStringAsync();
            errorMessage.Should().Contain("Types not found");
        }

        [Fact]
        public async Task SearchListTypesTips_ShouldReturnOk_WhenTypesMatchCriteria()
        {
            var id = 100;
            await SeedDocumentAsync(id, type:"Health");
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/types_tips/searchList?Type=Health&pageNumber=1&pageSize=5";
            
            using var response = await _client.GetAsync(endpoint);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<PaginatedResult<Types_TipsDTO>>();
            content.Should().NotBeNull();
            content.TotalCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task SearchListTypesTips_ShouldReturnNotFound_WhenNoTypesMatchCriteria()
        {
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/types_tips/searchList?Type=nonexistent&pageNumber=1&pageSize=5";
            
            using var response = await _client.GetAsync(endpoint);
            
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var errorMessage = await response.Content.ReadAsStringAsync();
            errorMessage.Should().Contain("Types not found");
        }

        [Fact]
        public async Task SearchTypeTip_ShouldReturnOk_WhenTypeExists()
        {
            var id = 3;
            await SeedDocumentAsync(id);
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/types_tips/search?TypeId=3";
            
            using var response = await _client.GetAsync(endpoint);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<Types_TipsDTO>();
            content.Should().NotBeNull();
            content.TypeId.Should().Be(3);
        }

        [Fact]
        public async Task SearchTypeTip_ShouldReturnNotFound_WhenTypeDoesNotExist()
        {
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/types_tips/search?TypeId=9999";
            
            using var response = await _client.GetAsync(endpoint);
            
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var errorMessage = await response.Content.ReadAsStringAsync();
            errorMessage.Should().Contain("Types not found");
        }

        [Fact]
        public async Task AddTypeTip_ShouldReturnCreated_WhenTypeIsValid()
        {
            var id = 100;
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/types_tips";
            var typeTip = new Types_TipsDTO
            {
                TypeId = id, 
                Type = "Health"
            };
            
            using var response = await _client.PostAsJsonAsync(endpoint, typeTip);
            
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var saved = await QuerySender.QueryDbAsync(_factory,db => db.Types_TipsDb
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Type == "Health"));

            saved.Should().NotBeNull();
            saved.Type.Should().Be("Health");
        }

        [Fact]
        public async Task UpdateTypeTip_ShouldReturnOk_WhenTypeIsValid()
        {
            var id = 100;
            await SeedDocumentAsync(id);
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/types_tips";
            var typeTip = new Types_TipsDTO
            {
                TypeId = id, 
                Type = "Updated Health"
            };
            
            using var response = await _client.PutAsJsonAsync(endpoint, typeTip);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var successMessage = await response.Content.ReadAsStringAsync();
            successMessage.Should().Contain("Type updated successfully");
            
            var updated = await QuerySender.QueryDbAsync(_factory,db => db.Types_TipsDb
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.TypeId == id));
            
            updated.Should().NotBeNull();
            updated.Type.Should().Be("Updated Health");
        }

        [Fact]
        public async Task UpdateTypeTip_ShouldReturnBadRequest_WhenTypeIsInvalid()
        {
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/types_tips";
            var typeTip = new Types_TipsDTO
            {
                TypeId = 0, 
                Type = ""
            };
            
            using var response = await _client.PutAsJsonAsync(endpoint, typeTip);
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteTypeTip_ShouldReturnOk_WhenTypeExists()
        {
            var id = 100;
            await SeedDocumentAsync(id);
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/types_tips/{id}";
 
            using var response = await _client.DeleteAsync(endpoint);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var successMessage = await response.Content.ReadAsStringAsync();
            successMessage.Should().Contain("Type deleted successfully");
        }

        [Fact]
        public async Task DeleteTypeTip_ShouldReturnBadRequest_WhenTypeDoesNotExist()
        {
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/types_tips/9999";
            
            using var response = await _client.DeleteAsync(endpoint);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        private async Task<Types_TipsDocument> SeedDocumentAsync(
            int typeId = 1,
            string type = "test")
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CalmskaDbContext>();

            var tt = new Types_TipsDocument
            {
                TypeId = typeId,
                Type = type
            };

            await db.Types_TipsDb.AddAsync(tt);
            await db.SaveChangesAsync();

            return tt;
        }
    }
}
