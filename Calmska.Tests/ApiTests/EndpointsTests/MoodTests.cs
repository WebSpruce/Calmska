namespace Calmska.Tests.ApiTests.EndpointsTests
{
    public class MoodTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public MoodTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllMoods_ShouldReturnOk_WhenMoodsExist()
        {
            string endpoint = "/api/v1/moods";

            var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadFromJsonAsync<PaginatedResult<Mood>>();
            content.Should().NotBeNull();
            content.TotalCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task SearchMoods_ShouldReturnOk_WhenMoodExists()
        {
            string endpoint = "/api/v1/moods/search?MoodId=9944e640-9504-47d5-943d-2d7750d909d5";

            var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadFromJsonAsync<Mood>();
            content.Should().NotBeNull();
            content.MoodId.Should().Be(Guid.Parse("9944e640-9504-47d5-943d-2d7750d909d5"));
        }

        [Fact]
        public async Task SearchMoods_ShouldReturnNotFound_WhenMoodDoesNotExist()
        {
            string endpoint = "/api/v1/moods/search?MoodId=00000000-0000-0000-0000-000000000000";

            var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task AddMood_ShouldReturnCreated_WhenMoodIsValid()
        {
            string endpoint = "/api/v1/moods";
            var mood = new MoodDTO
            {
                MoodId = Guid.Parse("44a85f64-5717-4562-b3fc-2c963f66afa6"),
                MoodName = "Happy",
                Type = "Positive"
            };

            var response = await _client.PostAsJsonAsync(endpoint, mood);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var location = response.Headers.Location.ToString();
            location.Should().Contain(mood.MoodId.ToString());
        }

        [Fact]
        public async Task UpdateMood_ShouldReturnOk_WhenMoodIsUpdated()
        {
            string endpoint = "/api/v1/moods";
            var mood = new MoodDTO
            {
                MoodId = Guid.Parse("44a85f64-5717-4562-b3fc-2c963f66afa6"),
                MoodName = "Excited",
                Type = "Positive"
            };

            var response = await _client.PutAsJsonAsync(endpoint, mood);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteMood_ShouldReturnOk_WhenMoodExists()
        {
            string endpoint = "/api/v1/moods";
            var moodId = Guid.Parse("44a85f64-5717-4562-b3fc-2c963f66afa6");

            var response = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(endpoint, UriKind.Relative),
                Content = JsonContent.Create(moodId)
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteMood_ShouldReturnBadRequest_WhenMoodDoesNotExist()
        {
            string endpoint = "/api/v1/moods";
            var moodId = Guid.Parse("00000000-0000-0000-0000-000000000000");

            var response = await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(endpoint, UriKind.Relative),
                Content = JsonContent.Create(moodId)
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
