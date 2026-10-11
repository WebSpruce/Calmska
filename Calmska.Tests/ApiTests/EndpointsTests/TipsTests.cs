using Calmska.Api.Endpoints;
using Calmska.Application.DTO;
using Calmska.Domain.Common;
using Calmska.Infrastructure.Persistence;
using Calmska.Infrastructure.Persistence.Models;
using Calmska.Tests.ApiTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Calmska.Tests.ApiTests.EndpointsTests
{
    public class TipsTests : IClassFixture<CalmskaApiFactory>
    {
        private readonly CalmskaApiFactory _factory;
        private readonly HttpClient _client;
        public TipsTests(CalmskaApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }
        [Fact]
        public async Task GetAllTips_ShouldReturnOk_WhenTipsExist()
        {
            var id = Guid.NewGuid();
            await SeedDocumentAsync(id);
            var id2 = Guid.NewGuid();
            await SeedDocumentAsync(id2);
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/tips?pageNumber=1&pageSize=10";

            using var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<PaginatedResult<TipsDTO>>();
            content.Should().NotBeNull();
            content.TotalCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task SearchListTips_ShouldReturnOk_WhenTipsMatchCriteria()
        {
            var id = Guid.NewGuid();
            await SeedDocumentAsync(id, content:"Drink water regularly.", tipsTypeId:3);
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/tips/searchList?content=Drink water regularly.&type=3&pageNumber=1&pageSize=5";

            using var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<PaginatedResult<TipsDTO>>();
            content.Should().NotBeNull();
            content.TotalCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task SearchListTips_ShouldReturnNotFound_WhenNoTipsMatchCriteria()
        {
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/tips/searchList?content=nonexistent&type=1&pageNumber=1&pageSize=5";

            using var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var errorMessage = await response.Content.ReadAsStringAsync();
            errorMessage.Should().Contain("Tips not found");
        }
        [Fact]
        public async Task SearchTip_ShouldReturnOk_WhenTipExists()
        {
            var id = Guid.NewGuid();
            await SeedDocumentAsync(id);
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/tips/search?TipId={id}";

            using var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<TipsDTO>();
            content.Should().NotBeNull();
            content.TipId.Should().Be(id);
        }

        [Fact]
        public async Task SearchTip_ShouldReturnNotFound_WhenTipDoesNotExist()
        {
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/tips/search?TipId=00000000-0000-0000-0000-000000000000";

            using var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var errorMessage = await response.Content.ReadAsStringAsync();
            errorMessage.Should().Contain("Tip not found");
        }
        [Fact]
        public async Task AddTip_ShouldReturnCreated_WhenTipIsValid()
        {
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/tips";
            var tip = new TipsDTO
            {
                TipId = Guid.Parse("44a85f64-5717-4562-b3fc-2c963f66afa6"),
                Content = "Drink water regularly.",
                TipsTypeId = 3
            };

            using var response = await _client.PostAsJsonAsync(endpoint, tip);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var location = response.Headers.Location?.ToString();
            location.Should().Contain(tip.TipId.ToString());
            
            var saved = await QuerySender.QueryDbAsync(_factory,db => db.TipsDb
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Content == "Drink water regularly."));

            saved.Should().NotBeNull();
            saved.TipsTypeId.Should().Be(3);
        }
        [Fact]
        public async Task UpdateTip_ShouldReturnOk_WhenTipIsValid()
        {
            var id = Guid.NewGuid();
            await SeedDocumentAsync(id);
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/tips";
            var tip = new TipsDTO
            {
                TipId = id,
                Content = "Updated tip content.",
                TipsTypeId = 4
            };

            using var response = await _client.PutAsJsonAsync(endpoint, tip);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var successMessage = await response.Content.ReadAsStringAsync();
            successMessage.Should().Contain("Tip updated successfully");
            
            var updated = await QuerySender.QueryDbAsync(_factory,db => db.TipsDb
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.TipId == id));
            
            updated.Should().NotBeNull();
            updated.Content.Should().Be("Updated tip content.");
            updated.TipsTypeId.Should().Be(4);
        }

        [Fact]
        public async Task UpdateTip_ShouldReturnBadRequest_WhenTipIsInvalid()
        {
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/tips";
            var tip = new TipsDTO
            {
                TipId = Guid.Empty,
                Content = string.Empty, 
                TipsTypeId = null
            };

            using var response = await _client.PutAsJsonAsync(endpoint, tip);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task DeleteTip_ShouldReturnOk_WhenTipExists()
        {
            var id = Guid.NewGuid();
            await SeedDocumentAsync(id);
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/tips/{id}";
            
            using var response = await _client.DeleteAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var successMessage = await response.Content.ReadAsStringAsync();
            successMessage.Should().Contain("Tip deleted successfully");
        }

        [Fact]
        public async Task DeleteTip_ShouldReturnBadRequest_WhenTipDoesNotExist()
        {
            var id = Guid.NewGuid();
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/tips/{id}";

            using var response = await _client.DeleteAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private async Task<TipsDocument> SeedDocumentAsync(
            Guid? tipId = null,
            string content = "Test",
            int tipsTypeId = 1
            )
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CalmskaDbContext>();

            var tip = new TipsDocument()
            {
                TipId = tipId ?? Guid.NewGuid(),
                Content = content,
                TipsTypeId = tipsTypeId
            };

            await db.TipsDb.AddAsync(tip);
            await db.SaveChangesAsync();

            return tip;
        }
    }
}
