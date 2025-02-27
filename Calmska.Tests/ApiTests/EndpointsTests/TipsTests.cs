namespace Calmska.Tests.ApiTests.EndpointsTests
{
    public class TipsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public TipsTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }
        [Fact]
        public async Task GetAllTips_ShouldReturnOk_WhenTipsExist()
        {
            string endpoint = "/api/v2/tips?pageNumber=1&pageSize=10";

            var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<PaginatedResult<TipsDTO>>();
            content.Should().NotBeNull();
            content.TotalCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task SearchListTips_ShouldReturnOk_WhenTipsMatchCriteria()
        {
            string endpoint = "/api/v2/tips/searchList?Content=Updated tip content.&Type=updated-type&pageNumber=1&pageSize=5";

            var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<PaginatedResult<TipsDTO>>();
            content.Should().NotBeNull();
            content.TotalCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task SearchListTips_ShouldReturnNotFound_WhenNoTipsMatchCriteria()
        {
            string endpoint = "/api/v2/tips/searchList?Content=nonexistent&type=random&pageNumber=1&pageSize=5";

            var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var errorMessage = await response.Content.ReadAsStringAsync();
            errorMessage.Should().Contain("Tips not found");
        }
        [Fact]
        public async Task SearchTip_ShouldReturnOk_WhenTipExists()
        {
            string endpoint = "/api/v2/tips/search?TipId=44a85f64-5717-4562-b3fc-2c963f66afa6";

            var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<TipsDTO>();
            content.Should().NotBeNull();
            content.TipId.Should().Be(Guid.Parse("44a85f64-5717-4562-b3fc-2c963f66afa6"));
        }

        [Fact]
        public async Task SearchTip_ShouldReturnNotFound_WhenTipDoesNotExist()
        {
            string endpoint = "/api/v2/tips/search?TipId=00000000-0000-0000-0000-000000000000";

            var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var errorMessage = await response.Content.ReadAsStringAsync();
            errorMessage.Should().Contain("Tip not found");
        }
        [Fact]
        public async Task AddTip_ShouldReturnCreated_WhenTipIsValid()
        {
            string endpoint = "/api/v2/tips";
            var tip = new TipsDTO
            {
                TipId = Guid.Parse("44a85f64-5717-4562-b3fc-2c963f66afa6"),
                Content = "Drink water regularly.",
                TipsTypeId = 3
            };

            var response = await _client.PostAsJsonAsync(endpoint, tip);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var location = response.Headers.Location.ToString();
            location.Should().Contain(tip.TipId.ToString());
        }
        [Fact]
        public async Task UpdateTip_ShouldReturnOk_WhenTipIsValid()
        {
            string endpoint = "/api/v2/tips";
            var tip = new TipsDTO
            {
                TipId = Guid.Parse("44a85f64-5717-4562-b3fc-2c963f66afa6"),
                Content = "Updated tip content.",
                TipsTypeId = 4
            };

            var response = await _client.PutAsJsonAsync(endpoint, tip);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var successMessage = await response.Content.ReadAsStringAsync();
            successMessage.Should().Contain("Tip updated successfully");
        }

        [Fact]
        public async Task UpdateTip_ShouldReturnBadRequest_WhenTipIsInvalid()
        {
            string endpoint = "/api/v2/tips";
            var tip = new TipsDTO
            {
                TipId = Guid.Empty,
                Content = string.Empty, 
                TipsTypeId = null
            };

            var response = await _client.PutAsJsonAsync(endpoint, tip);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task DeleteTip_ShouldReturnOk_WhenTipExists()
        {
            string endpoint = "/api/v2/tips";
            Guid tipId = Guid.Parse("44a85f64-5717-4562-b3fc-2c963f66afa6");

            var request = new HttpRequestMessage(HttpMethod.Delete, endpoint)
            {
                Content = JsonContent.Create(tipId)
            };
            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var successMessage = await response.Content.ReadAsStringAsync();
            successMessage.Should().Contain("Tip deleted successfully");
        }

        [Fact]
        public async Task DeleteTip_ShouldReturnBadRequest_WhenTipDoesNotExist()
        {
            string endpoint = "/api/v2/tips";
            Guid tipId = Guid.NewGuid();

            var request = new HttpRequestMessage(HttpMethod.Delete, endpoint)
            {
                Content = JsonContent.Create(tipId)
            };
            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


    }
}
