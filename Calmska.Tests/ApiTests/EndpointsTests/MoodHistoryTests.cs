using Calmska.Api.Endpoints;
using Calmska.Application.DTO;
using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Infrastructure.Persistence;
using Calmska.Infrastructure.Persistence.Models;
using Calmska.Tests.ApiTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Calmska.Tests.ApiTests.EndpointsTests
{
    public class MoodHistoryTests : IClassFixture<CalmskaApiFactory>
    {
        private readonly CalmskaApiFactory _factory;
        private readonly HttpClient _client;

        public MoodHistoryTests(CalmskaApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllMoodHistory_ShouldReturnOk_WhenMoodHistoryExists()
        {
            await SeedDocumentAsync(Guid.NewGuid());
            await SeedDocumentAsync(Guid.NewGuid());
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/moodhistory";

            using var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<PaginatedResult<MoodHistory>>();
            content.Should().NotBeNull();
            content.TotalCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task SearchMoodHistory_ShouldReturnBadRequest_WhenDateFormatIsInvalid()
        {
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/moodhistory/search?Date=invalid-date";

            using var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SearchMoodHistory_ShouldReturnOk_WhenMoodHistoryExists()
        {
            var id = Guid.Parse("a6e1cb88-0c15-43b3-a37f-076e14915d12");
            await SeedDocumentAsync(id);
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/moodhistory/search?MoodHistoryId={id}";

            using var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<MoodHistory>();
            content.Should().NotBeNull();
            content.MoodHistoryId.Should().Be(id);
        }

        [Fact]
        public async Task AddMoodHistory_ShouldReturnCreated_WhenMoodHistoryIsValid()
        {
            var userId = Guid.NewGuid();
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/moodhistory";
            var moodHistory = new MoodHistoryDTO
            {
                MoodHistoryId = Guid.Parse("532af909-6d0e-4d6e-b5e9-f1f49d577a9f"),
                UserId = userId,
                MoodId = Guid.Parse("532af909-6d0e-4d6e-b5e9-f1f49d577a9f"),
                Date = DateTime.UtcNow
            };

            using var response = await _client.PostAsJsonAsync(endpoint, moodHistory);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var location = response.Headers.Location?.ToString();
            location.Should().Contain(moodHistory.UserId.ToString());
            
            var saved = await QuerySender.QueryDbAsync(_factory,db => db.MoodHistoryDb
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.UserId == userId));

            saved.Should().NotBeNull();
            saved.MoodId.Should().Be("532af909-6d0e-4d6e-b5e9-f1f49d577a9f");
        }

        [Fact]
        public async Task UpdateMoodHistory_ShouldReturnOk_WhenMoodHistoryIsValid()
        {
            var moodId = Guid.Parse("c03bee91-94ec-4016-912b-8743715bfce6");
            var userId = Guid.Parse("c03bee91-94ec-4016-912b-8743715bfce6");
            var id = Guid.Parse("a6e1cb88-0c15-43b3-a37f-076e14915d12");
            await SeedDocumentAsync(id);
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/moodhistory";
            var moodHistory = new MoodHistoryDTO
            {
                MoodHistoryId = id,
                UserId = userId,
                MoodId = moodId,
                Date = DateTime.UtcNow
            };

            using var response = await _client.PutAsJsonAsync(endpoint, moodHistory);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var updated = await QuerySender.QueryDbAsync(_factory,db => db.MoodHistoryDb
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.MoodHistoryId == id));
            
            updated.Should().NotBeNull();
            updated.UserId.Should().Be(userId);
            updated.MoodId.Should().Be(moodId);
        }

        [Fact]
        public async Task DeleteMoodHistory_ShouldReturnOk_WhenMoodHistoryExists()
        {
            var id = Guid.Parse("a6e1cb88-0c15-43b3-a37f-076e14915d12");
            await SeedDocumentAsync(id);
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/moodhistory/{id}";
            
            using var response = await _client.DeleteAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var deleted = await QuerySender.QueryDbAsync(_factory, db => db.MoodHistoryDb
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.MoodHistoryId == id));
            deleted.Should().BeNull();
        }

        [Fact]
        public async Task DeleteMoodHistory_ShouldReturnBadRequest_WhenMoodHistoryDoesNotExist()
        {
            Guid moodHistoryId = Guid.NewGuid();
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/moodhistory/{moodHistoryId}";

            using var response = await _client.DeleteAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        private async Task<MoodHistoryDocument> SeedDocumentAsync(
            Guid? moodHistoryId = null,
            Guid? moodId = null
        )
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CalmskaDbContext>();

            var moodHistory = new MoodHistoryDocument()
            {
                MoodHistoryId = moodHistoryId ?? Guid.NewGuid(),
                MoodId = moodId ?? Guid.NewGuid(),
                Date = DateTime.UtcNow,
                UserId = Guid.NewGuid()
            };

            await db.MoodHistoryDb.AddAsync(moodHistory);
            await db.SaveChangesAsync();

            return moodHistory;
        }
    }
}
