namespace Calmska.Tests.ApiTests.EndpointsTests
{
    public class SettingsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public SettingsTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }
        [Fact]
        public async Task GetAllSettings_ShouldReturnOk_WhenSettingsExist()
        {
            string endpoint = "/api/v2/settings";

            var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadFromJsonAsync<PaginatedResult<Settings>>();
            content.Should().NotBeNull();
            content.TotalCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetSettingsBySearch_ShouldReturnOk_WhenSettingMatchesSearchCriteria()
        {
            string endpoint = "/api/v2/settings/search?Color=Red";

            var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadFromJsonAsync<Settings>();
            content.Should().NotBeNull();
            content.Color.Should().Be("Red");
        }

        [Fact]
        public async Task GetSettingsBySearch_ShouldReturnNotFound_WhenSettingDoesNotExist()
        {
            string endpoint = "/api/v2/settings/search?SettingsId=00000000-0000-0000-0000-000000000000";

            var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetSettingsList_ShouldReturnOk_WhenMatchingSettingsExist()
        {
            string endpoint = "/api/v2/settings/searchList?PomodoroTimer=25";

            var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadFromJsonAsync<PaginatedResult<Settings>>();
            content.Should().NotBeNull();
            content.TotalCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task AddSetting_ShouldReturnCreated_WhenSettingIsValid()
        {
            string endpoint = "/api/v2/settings";
            var setting = new SettingsDTO
            {
                SettingsId = Guid.Parse("44a85f64-5717-4562-b3fc-2c963f66afa6"),
                UserId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                Color = "Green",
                PomodoroTimer = "25",
                PomodoroBreak = "5"
            };

            var response = await _client.PostAsJsonAsync(endpoint, setting);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var location = response.Headers.Location.ToString();
            location.Should().Contain(setting.SettingsId.ToString());
        }

        [Fact]
        public async Task UpdateSetting_ShouldReturnOk_WhenSettingIsUpdated()
        {
            string endpoint = "/api/v2/settings";
            var setting = new SettingsDTO
            {
                SettingsId = Guid.Parse("44a85f64-5717-4562-b3fc-2c963f66afa6"),
                UserId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                Color = "Red",
                PomodoroTimer = "30",
                PomodoroBreak = "10"
            };

            var response = await _client.PutAsJsonAsync(endpoint, setting);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteSetting_ShouldReturnOk_WhenSettingExists()
        {
            string endpoint = "/api/v2/settings";

            var settingsId = Guid.Parse("44a85f64-5717-4562-b3fc-2c963f66afa6");
            var response = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(endpoint, UriKind.Relative),
                Content = JsonContent.Create(settingsId)
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
