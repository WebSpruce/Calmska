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
    public class MoodTests : IClassFixture<CalmskaApiFactory>
    {
        private readonly CalmskaApiFactory _factory;
        private readonly HttpClient _client;

        public MoodTests(CalmskaApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllMoods_ShouldReturnOk_WhenMoodsExist()
        {
            await SeedDocumentAsync(moodId:Guid.NewGuid());
            await SeedDocumentAsync(moodId:Guid.NewGuid());
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/moods";

            using var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadFromJsonAsync<PaginatedResult<Mood>>();
            content.Should().NotBeNull();
            content.TotalCount.Should().BeGreaterThan(0);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task SearchMoods_ShouldReturnOk_WhenMoodExists()
        {
            var moodId = Guid.Parse("9944e640-9504-47d5-943d-2d7750d909d5");
            await SeedDocumentAsync(moodId:moodId);
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/moods/search?MoodId=9944e640-9504-47d5-943d-2d7750d909d5";

            using var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadFromJsonAsync<Mood>();
            content.Should().NotBeNull();
            content.MoodId.Should().Be(Guid.Parse("9944e640-9504-47d5-943d-2d7750d909d5"));
        }

        [Fact]
        public async Task SearchMoods_ShouldReturnNotFound_WhenMoodDoesNotExist()
        {
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/moods/search?MoodId=00000000-0000-0000-0000-000000000000";

            using var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task AddMood_ShouldReturnCreated_WhenMoodIsValid()
        {
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/moods";
            var mood = new MoodDTO
            {
                MoodId = Guid.Parse("9944e640-9504-47d5-943d-2d7750d90999"),
                MoodName = "Happy",
                MoodTypeId = 3
            };

            using var response = await _client.PostAsJsonAsync(endpoint, mood);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var location = response.Headers.Location?.ToString();
            location.Should().Contain(mood.MoodId.ToString());
            
            var saved = await QuerySender.QueryDbAsync(_factory,db => db.Moods
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.MoodName == mood.MoodName && x.MoodTypeId == mood.MoodTypeId));

            saved.Should().NotBeNull();
            saved!.MoodName.Should().Be("Happy");
            saved.MoodTypeId.Should().Be(3);
        }

        [Fact]
        public async Task UpdateMood_ShouldReturnOk_WhenMoodIsUpdated()
        {
            var moodId = Guid.Parse("44a85f64-5717-4562-b3fc-2c963f66afa6");
            await SeedDocumentAsync(moodId:moodId);
            
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/moods";
            var mood = new MoodDTO
            {
                MoodId = moodId,
                MoodName = "Excited",
                MoodTypeId = 3
            };

            using var response = await _client.PutAsJsonAsync(endpoint, mood);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var updated = await QuerySender.QueryDbAsync(_factory,db => 
                db.Moods.AsNoTracking().SingleOrDefaultAsync(x => x.MoodId == moodId));
            
            updated.Should().NotBeNull();
            updated!.MoodName.Should().Be("Excited");
            updated.MoodTypeId.Should().Be(3);
        }

        [Fact]
        public async Task DeleteMood_ShouldReturnOk_WhenMoodExists()
        {
            var moodId = Guid.Parse("44a85f64-5717-4562-b3fc-2c963f66afa6");
            await SeedDocumentAsync(moodId:moodId);
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/moods/{moodId}";

            using var response = await _client.DeleteAsync(endpoint);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var deleted = await QuerySender.QueryDbAsync(_factory, 
                db => db.Moods.AsNoTracking().SingleOrDefaultAsync(x => x.MoodId == moodId));
            deleted.Should().BeNull();
        }

        [Fact]
        public async Task DeleteMood_ShouldReturnBadRequest_WhenMoodDoesNotExist()
        {
            var moodId = Guid.Parse("44a85f64-5717-4562-b3fc-2c963f66afa6");
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/moods/{moodId}";

            using var response = await _client.DeleteAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        private async Task<MoodDocument> SeedDocumentAsync(
            Guid? moodId = null,
            int moodTypeId = 1,
            string moodName = "Test"
            )
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CalmskaDbContext>();

            var mood = new MoodDocument
            {
                MoodId = moodId ?? Guid.NewGuid(),
                MoodName = moodName,
                MoodTypeId = moodTypeId
            };

            await db.Moods.AddAsync(mood);
            await db.SaveChangesAsync();

            return mood;
        }
    }
}
