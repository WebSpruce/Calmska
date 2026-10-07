using Calmska.Api.Endpoints;
using Calmska.Application.DTO;
using Calmska.Domain.Common;
using Calmska.Infrastructure.Persistence;
using Calmska.Infrastructure.Persistence.Models;
using Calmska.Tests.ApiTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Calmska.Tests.ApiTests.EndpointsTests
{
    public class SettingsTests : IClassFixture<CalmskaApiFactory>
    {
        private readonly CalmskaApiFactory _factory;
        private readonly HttpClient _client;
        public SettingsTests(CalmskaApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }
        [Fact]
        public async Task GetAllSettings_ShouldReturnOk_WhenSettingsExist()
        {
            await SeedSettingsAsync(Guid.CreateVersion7(), Guid.CreateVersion7());
            await SeedSettingsAsync(Guid.CreateVersion7(), Guid.CreateVersion7());
            
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/settings";

            var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadFromJsonAsync<PaginatedResult<SettingsDTO>>();
            content.Should().NotBeNull();
            content.TotalCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetSettingsBySearch_ShouldReturnOk_WhenSettingMatchesSearchCriteria()
        {
            await SeedSettingsAsync(Guid.CreateVersion7(), Guid.CreateVersion7(), color:"Red");
            await SeedSettingsAsync(Guid.CreateVersion7(), Guid.CreateVersion7());
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/settings/search?Color=Red";

            var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadFromJsonAsync<SettingsDTO>();
            content.Should().NotBeNull();
            content.Color.Should().Be("Red");
        }

        [Fact]
        public async Task GetSettingsBySearch_ShouldReturnNotFound_WhenSettingDoesNotExist()
        {
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/settings/search?SettingsId=00000000-0000-0000-0000-000000000000";

            var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetSettingsList_ShouldReturnOk_WhenMatchingSettingsExist()
        {
            await SeedSettingsAsync(Guid.CreateVersion7(), Guid.CreateVersion7(), timer: "30");
            await SeedSettingsAsync(Guid.CreateVersion7(), Guid.CreateVersion7());
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/settings/searchList?pomodoroTimer=30";

            var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadFromJsonAsync<PaginatedResult<SettingsDTO>>();
            content.Should().NotBeNull();
            content.TotalCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task AddSetting_ShouldReturnCreated_WhenSettingIsValid()
        {
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/settings";
            var setting = new SettingsDTO
            {
                SettingsId = Guid.CreateVersion7(),
                UserId = Guid.CreateVersion7(),
                Color = "Green",
                PomodoroTimer = 25f,
                PomodoroBreak = 5f
            };

            using var response = await _client.PostAsJsonAsync(endpoint, setting);

            var body = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.Created, because: body);

            response.Headers.Location.Should().NotBeNull();
            response.Headers.Location!.ToString().Should().Contain(setting.SettingsId.ToString());

            var saved = await QuerySender.QueryDbAsync(_factory,db => db.SettingsDb
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.UserId == setting.UserId));

            saved.Should().NotBeNull();
            saved!.Color.Should().Be("Green");
            saved.PomodoroTimer.Should().Be("25");
            saved.PomodoroBreak.Should().Be("5");
        }

        [Fact]
        public async Task UpdateSetting_ShouldReturnOk_WhenSettingIsUpdated()
        {
            var settingId = Guid.Parse("c03bee91-94ec-4016-912b-8743715bfce6");
            var userId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            await SeedSettingsAsync(settingId, userId);
            
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/settings";
            var setting = new SettingsDTO
            {
                SettingsId = settingId,
                UserId = userId,
                Color = "Red",
                PomodoroTimer = 30f,
                PomodoroBreak = 10f
            };
                                                                                                                                                                   
            using var response = await _client.PutAsJsonAsync(endpoint, setting);
                                                                                                                                                             
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var updated = await QuerySender.QueryDbAsync(_factory,db => db.SettingsDb.AsNoTracking().SingleOrDefaultAsync(x => x.SettingsId == settingId));
            
            updated.Should().NotBeNull();
            updated!.Color.Should().Be("Red");
            updated.PomodoroTimer.Should().Be("30");
        }

        [Fact]
        public async Task DeleteSetting_ShouldReturnOk_WhenSettingExists()
        {
            var settingId = Guid.Parse("44a85f64-5717-4562-b3fc-2c963f66afa6");
            var userId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            await SeedSettingsAsync(settingId, userId);
            
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/settings/{settingId}";

            using var response = await _client.DeleteAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var deleted = await QuerySender.QueryDbAsync(_factory, db => db.SettingsDb.AsNoTracking().SingleOrDefaultAsync(x => x.SettingsId == settingId));
            deleted.Should().BeNull();
        }
        
        private async Task<SettingsDocument> SeedSettingsAsync(
            Guid? settingsId = null,
            Guid? userId = null,
            string color = "Green",
            string timer = "25",
            string pomodoroBreak = "5")
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CalmskaDbContext>();

            var setting = new SettingsDocument
            {
                SettingsId = settingsId ?? Guid.NewGuid(),
                UserId = userId ?? Guid.NewGuid(),
                Color = color,
                PomodoroTimer = timer,
                PomodoroBreak = pomodoroBreak
            };

            await db.SettingsDb.AddAsync(setting);
            await db.SaveChangesAsync();

            return setting;
        }
    }
}
