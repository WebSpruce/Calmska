namespace Calmska.Tests.ServicesTests
{
    public class SettingsServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly SettingsService _settingsService;

        public SettingsServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _settingsService = new SettingsService(_httpClient);
        }

        private void SetupHttpResponse(HttpStatusCode statusCode, string content = "")
        {
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                });
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnSettings_WhenRequestIsSuccessful()
        {
            var expectedData = new List<SettingsDTO?>
            {
                new SettingsDTO { SettingsId = Guid.NewGuid(), Color = "Blue", PomodoroBreakFloat = 5, PomodoroTimerFloat = 25, UserId = Guid.NewGuid() },
                new SettingsDTO { SettingsId = Guid.NewGuid(), Color = "Red", PomodoroBreakFloat = 10, PomodoroTimerFloat = 30, UserId = Guid.NewGuid() }
            };

            SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(expectedData));

            var result = await _settingsService.GetAllAsync(null, null);

            Assert.NotNull(result);
            Assert.True(result.Result != null && result.Result.Any());
            Assert.Equal(string.Empty, result.Error);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnError_WhenRequestFails()
        {
            SetupHttpResponse(HttpStatusCode.BadRequest, "Bad Request");

            var result = await _settingsService.GetAllAsync(null, null);

            Assert.NotNull(result);
            Assert.Null(result.Result);
            Assert.Contains("Request failed with status code", result.Error);
        }

        [Fact]
        public async Task SearchAllByArgumentAsync_ShouldReturnPaginatedSettings_WhenRequestIsSuccessful()
        {
            var settingsCriteria = new SettingsDTO { Color = "Blue" };
            var expectedData = new PaginatedResult<IEnumerable<SettingsDTO?>>
            {
                Items = new List<IEnumerable<SettingsDTO?>>
                {
                    new List<SettingsDTO?>
                    {
                        new SettingsDTO { SettingsId = Guid.NewGuid(), Color = "Blue", PomodoroBreakFloat = 5, PomodoroTimerFloat = 25, UserId = Guid.NewGuid() }
                    }
                },
                TotalCount = 1
            };

            SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(expectedData));

            var result = await _settingsService.SearchAllByArgumentAsync(settingsCriteria, null, null);

            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.Equal(1, result.Result.TotalCount);
            Assert.Equal(string.Empty, result.Error);
        }

        [Fact]
        public async Task GetByArgumentAsync_ShouldReturnSettings_WhenRequestIsSuccessful()
        {
            var settingsCriteria = new SettingsDTO { SettingsId = Guid.NewGuid() };
            var expectedSettings = new SettingsDTO { SettingsId = settingsCriteria.SettingsId, Color = "Green", PomodoroBreakFloat = 10, PomodoroTimerFloat = 30, UserId = Guid.NewGuid() };

            SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(expectedSettings));

            var result = await _settingsService.GetByArgumentAsync(settingsCriteria);

            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.Equal("Green", result.Result.Color);
            Assert.Equal(string.Empty, result.Error);
        }

        [Fact]
        public async Task AddAsync_ShouldReturnSuccess_WhenRequestIsSuccessful()
        {
            var newSettings = new SettingsDTO { SettingsId = Guid.NewGuid(), Color = "Yellow", PomodoroBreakFloat = 5, PomodoroTimerFloat = 25, UserId = Guid.NewGuid() };

            SetupHttpResponse(HttpStatusCode.Created);

            var result = await _settingsService.AddAsync(newSettings);

            Assert.NotNull(result);
            Assert.True(result.Result);
            Assert.Equal(string.Empty, result.Error);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenRequestIsSuccessful()
        {
            var updatedSettings = new SettingsDTO { SettingsId = Guid.NewGuid(), Color = "Purple", PomodoroBreakFloat = 15, PomodoroTimerFloat = 40, UserId = Guid.NewGuid() };

            SetupHttpResponse(HttpStatusCode.OK);

            var result = await _settingsService.UpdateAsync(updatedSettings);

            Assert.NotNull(result);
            Assert.True(result.Result);
            Assert.Equal(string.Empty, result.Error);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenRequestIsSuccessful()
        {
            var settingsId = Guid.NewGuid();

            SetupHttpResponse(HttpStatusCode.OK);

            var result = await _settingsService.DeleteAsync(settingsId);

            Assert.NotNull(result);
            Assert.True(result.Result);
            Assert.Equal(string.Empty, result.Error);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenRequestFails()
        {
            var settingsId = Guid.NewGuid();

            SetupHttpResponse(HttpStatusCode.BadRequest, "Bad Request");

            var result = await _settingsService.DeleteAsync(settingsId);

            Assert.NotNull(result);
            Assert.False(result.Result);
            Assert.Contains("Request failed with status code", result.Error);
        }
    }
}
