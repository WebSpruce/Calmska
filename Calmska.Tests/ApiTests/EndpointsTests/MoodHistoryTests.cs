namespace Calmska.Tests.ApiTests.EndpointsTests
{
    public class MoodHistoryTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public MoodHistoryTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllMoodHistory_ShouldReturnOk_WhenMoodHistoryExists()
        {
            string endpoint = "/api/v2/moodhistory";

            var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<PaginatedResult<MoodHistory>>();
            content.Should().NotBeNull();
            content.TotalCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task SearchMoodHistory_ShouldReturnBadRequest_WhenDateFormatIsInvalid()
        {
            string endpoint = "/api/v2/moodhistory/search?Date=invalid-date";

            var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SearchMoodHistory_ShouldReturnOk_WhenMoodHistoryExists()
        {
            string endpoint = "/api/v2/moodhistory/search?MoodHistoryId=532af909-6d0e-4d6e-b5e9-f1f49d577a9f";

            var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<MoodHistory>();
            content.Should().NotBeNull();
            content.MoodHistoryId.Should().Be(Guid.Parse("532af909-6d0e-4d6e-b5e9-f1f49d577a9f"));
        }

        [Fact]
        public async Task AddMoodHistory_ShouldReturnCreated_WhenMoodHistoryIsValid()
        {
            string endpoint = "/api/v2/moodhistory";
            var moodHistory = new MoodHistoryDTO
            {
                MoodHistoryId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                MoodId = Guid.NewGuid(),
                Date = DateTime.UtcNow
            };

            var response = await _client.PostAsJsonAsync(endpoint, moodHistory);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var location = response.Headers.Location.ToString();
            location.Should().Contain(moodHistory.UserId.ToString());
        }

        [Fact]
        public async Task UpdateMoodHistory_ShouldReturnOk_WhenMoodHistoryIsValid()
        {
            string endpoint = "/api/v2/moodhistory";
            var moodHistory = new MoodHistoryDTO
            {
                MoodHistoryId = Guid.Parse("532af909-6d0e-4d6e-b5e9-f1f49d577a9f"),
                UserId = Guid.NewGuid(),
                MoodId = Guid.NewGuid(),
                Date = DateTime.UtcNow
            };

            var response = await _client.PutAsJsonAsync(endpoint, moodHistory);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteMoodHistory_ShouldReturnOk_WhenMoodHistoryExists()
        {
            string endpoint = "/api/v2/moodhistory";
            Guid moodHistoryId = Guid.Parse("532af909-6d0e-4d6e-b5e9-f1f49d577a9f");

            var request = new HttpRequestMessage(HttpMethod.Delete, endpoint)
            {
                Content = JsonContent.Create(moodHistoryId)
            };
            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteMoodHistory_ShouldReturnBadRequest_WhenMoodHistoryDoesNotExist()
        {
            string endpoint = "/api/v2/moodhistory";
            Guid moodHistoryId = Guid.NewGuid();

            var request = new HttpRequestMessage(HttpMethod.Delete, endpoint)
            {
                Content = JsonContent.Create(moodHistoryId)
            };
            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

    }
}
