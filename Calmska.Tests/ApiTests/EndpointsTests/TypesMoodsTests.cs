namespace Calmska.Tests.ApiTests.EndpointsTests
{
    public class TypesMoodsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public TypesMoodsTests(WebApplicationFactory<Program> factory)
        {
            string ApiUrl = Environment.GetEnvironmentVariable("ApiUrl") ?? string.Empty;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllTypesMoods_ShouldReturnOk_WhenTypesExist()
        {
            string endpoint = "/api/v2/types_moods?pageNumber=1&pageSize=10";
            var response = await _client.GetAsync(endpoint);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<PaginatedResult<Types_MoodDTO>>();
            content.Should().NotBeNull();
            content.TotalCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetAllTypesMoods_ShouldReturnNotFound_WhenNoTypesExist()
        {
            string endpoint = "/api/v2/types_moods?pageNumber=1&pageSize=10";
            var response = await _client.GetAsync(endpoint);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var errorMessage = await response.Content.ReadAsStringAsync();
            errorMessage.Should().Contain("Types not found");
        }

        [Fact]
        public async Task SearchListTypesMoods_ShouldReturnOk_WhenTypesMatchCriteria()
        {
            string endpoint = "/api/v2/types_moods/searchList?Type=Calm&pageNumber=1&pageSize=5";
            var response = await _client.GetAsync(endpoint);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<PaginatedResult<Types_MoodDTO>>();
            content.Should().NotBeNull();
            content.TotalCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task SearchListTypesMoods_ShouldReturnNotFound_WhenNoTypesMatchCriteria()
        {
            string endpoint = "/api/v2/types_moods/searchList?Type=nonexistent&pageNumber=1&pageSize=5";
            var response = await _client.GetAsync(endpoint);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var errorMessage = await response.Content.ReadAsStringAsync();
            errorMessage.Should().Contain("Types not found");
        }

        [Fact]
        public async Task SearchTypeMood_ShouldReturnOk_WhenTypeExists()
        {
            string endpoint = "/api/v2/types_moods/search?TypeId=2";
            var response = await _client.GetAsync(endpoint);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<Types_MoodDTO>();
            content.Should().NotBeNull();
            content.TypeId.Should().Be(2);
        }

        [Fact]
        public async Task SearchTypeMood_ShouldReturnNotFound_WhenTypeDoesNotExist()
        {
            string endpoint = "/api/v2/types_moods/search?TypeId=9999";
            var response = await _client.GetAsync(endpoint);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var errorMessage = await response.Content.ReadAsStringAsync();
            errorMessage.Should().Contain("Types not found");
        }

        [Fact]
        public async Task AddTypeMood_ShouldReturnCreated_WhenTypeIsValid()
        {
            string endpoint = "/api/v2/types_moods";
            var typeMood = new Types_MoodDTO { TypeId = 2, Type = "Excited" };
            var response = await _client.PostAsJsonAsync(endpoint, typeMood);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task UpdateTypeMood_ShouldReturnOk_WhenTypeIsValid()
        {
            string endpoint = "/api/v2/types_moods";
            var typeMood = new Types_MoodDTO { TypeId = 2, Type = "Calm" };
            var response = await _client.PutAsJsonAsync(endpoint, typeMood);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var successMessage = await response.Content.ReadAsStringAsync();
            successMessage.Should().Contain("Type updated successfully");
        }

        [Fact]
        public async Task UpdateTypeMood_ShouldReturnBadRequest_WhenTypeIsInvalid()
        {
            string endpoint = "/api/v2/types_moods";
            var typeMood = new Types_MoodDTO { TypeId = 0, Type = "" };
            var response = await _client.PutAsJsonAsync(endpoint, typeMood);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteTypeMood_ShouldReturnOk_WhenTypeExists()
        {
            string endpoint = "/api/v2/types_moods";
            var request = new HttpRequestMessage(HttpMethod.Delete, endpoint)
            {
                Content = JsonContent.Create(2)
            };
            var response = await _client.SendAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var successMessage = await response.Content.ReadAsStringAsync();
            successMessage.Should().Contain("Type deleted successfully");
        }

        [Fact]
        public async Task DeleteTypeMood_ShouldReturnBadRequest_WhenTypeDoesNotExist()
        {
            string endpoint = "/api/v2/types_moods";
            var request = new HttpRequestMessage(HttpMethod.Delete, endpoint)
            {
                Content = JsonContent.Create(9999)
            };
            var response = await _client.SendAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
