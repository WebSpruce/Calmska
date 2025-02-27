namespace Calmska.Tests.ApiTests.EndpointsTests
{
    public class TypesTipsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public TypesTipsTests(WebApplicationFactory<Program> factory)
        {
            string ApiUrl = Environment.GetEnvironmentVariable("ApiUrl") ?? string.Empty;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllTypesTips_ShouldReturnOk_WhenTypesExist()
        {
            string endpoint = "/api/v2/types_tips?pageNumber=1&pageSize=10";
            var response = await _client.GetAsync(endpoint);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<PaginatedResult<Types_TipsDTO>>();
            content.Should().NotBeNull();
            content.TotalCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetAllTypesTips_ShouldReturnNotFound_WhenNoTypesExist()
        {
            string endpoint = "/api/v2/types_tips?pageNumber=1&pageSize=10";
            var response = await _client.GetAsync(endpoint);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var errorMessage = await response.Content.ReadAsStringAsync();
            errorMessage.Should().Contain("Types not found");
        }

        [Fact]
        public async Task SearchListTypesTips_ShouldReturnOk_WhenTypesMatchCriteria()
        {
            string endpoint = "/api/v2/types_tips/searchList?Type=some-type&pageNumber=1&pageSize=5";
            var response = await _client.GetAsync(endpoint);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<PaginatedResult<Types_TipsDTO>>();
            content.Should().NotBeNull();
            content.TotalCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task SearchListTypesTips_ShouldReturnNotFound_WhenNoTypesMatchCriteria()
        {
            string endpoint = "/api/v2/types_tips/searchList?Type=nonexistent&pageNumber=1&pageSize=5";
            var response = await _client.GetAsync(endpoint);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var errorMessage = await response.Content.ReadAsStringAsync();
            errorMessage.Should().Contain("Types not found");
        }

        [Fact]
        public async Task SearchTypeTip_ShouldReturnOk_WhenTypeExists()
        {
            string endpoint = "/api/v2/types_tips/search?TypeId=1";
            var response = await _client.GetAsync(endpoint);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<Types_TipsDTO>();
            content.Should().NotBeNull();
            content.TypeId.Should().Be(1);
        }

        [Fact]
        public async Task SearchTypeTip_ShouldReturnNotFound_WhenTypeDoesNotExist()
        {
            string endpoint = "/api/v2/types_tips/search?TypeId=9999";
            var response = await _client.GetAsync(endpoint);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var errorMessage = await response.Content.ReadAsStringAsync();
            errorMessage.Should().Contain("Types not found");
        }

        [Fact]
        public async Task AddTypeTip_ShouldReturnCreated_WhenTypeIsValid()
        {
            string endpoint = "/api/v2/types_tips";
            var typeTip = new Types_TipsDTO { TypeId = 1, Type = "Health" };
            var response = await _client.PostAsJsonAsync(endpoint, typeTip);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task UpdateTypeTip_ShouldReturnOk_WhenTypeIsValid()
        {
            string endpoint = "/api/v2/types_tips";
            var typeTip = new Types_TipsDTO { TypeId = 1, Type = "Updated Health" };
            var response = await _client.PutAsJsonAsync(endpoint, typeTip);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var successMessage = await response.Content.ReadAsStringAsync();
            successMessage.Should().Contain("Type updated successfully");
        }

        [Fact]
        public async Task UpdateTypeTip_ShouldReturnBadRequest_WhenTypeIsInvalid()
        {
            string endpoint = "/api/v2/types_tips";
            var typeTip = new Types_TipsDTO { TypeId = 0, Type = "" };
            var response = await _client.PutAsJsonAsync(endpoint, typeTip);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteTypeTip_ShouldReturnOk_WhenTypeExists()
        {
            string endpoint = "/api/v2/types_tips";
            var request = new HttpRequestMessage(HttpMethod.Delete, endpoint)
            {
                Content = JsonContent.Create(1)
            };
            var response = await _client.SendAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var successMessage = await response.Content.ReadAsStringAsync();
            successMessage.Should().Contain("Type deleted successfully");
        }

        [Fact]
        public async Task DeleteTypeTip_ShouldReturnBadRequest_WhenTypeDoesNotExist()
        {
            string endpoint = "/api/v2/types_tips";
            var request = new HttpRequestMessage(HttpMethod.Delete, endpoint)
            {
                Content = JsonContent.Create(9999)
            };
            var response = await _client.SendAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
