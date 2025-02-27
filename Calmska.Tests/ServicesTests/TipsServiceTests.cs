namespace Calmska.Tests.ServicesTests
{
    public class TipsServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly TipsService _tipsService;

        public TipsServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            var apiBaseUrl = "https://calmska.onrender.com/api/v2";
            _httpClient.BaseAddress = new Uri(apiBaseUrl);
            _tipsService = new TipsService(_httpClient);
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
        public async Task GetAllAsync_ShouldReturnTips_WhenRequestIsSuccessful()
        {
            var expectedData = new PaginatedResult<TipsDTO?> { 
                Items = new List<TipsDTO?>
                {
                    new TipsDTO { TipId = Guid.NewGuid(), Content = "Stay hydrated", TipsTypeId = 1 },
                    new TipsDTO { TipId = Guid.NewGuid(), Content = "Take breaks", TipsTypeId = 2 }
                },
                error = string.Empty,
                PageNumber = 1,
                PageSize = 2,
                TotalCount = 2
            };

            SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(expectedData));

            var result = await _tipsService.GetAllAsync(null, null);

            Assert.NotNull(result);
            Assert.True(result.Result != null && result.Result.Items != null && result.Result.Items.Any());
            Assert.Equal(string.Empty, result.Error);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnError_WhenRequestFails()
        {
            SetupHttpResponse(HttpStatusCode.BadRequest, "Bad Request");

            var result = await _tipsService.GetAllAsync(null, null);

            Assert.NotNull(result);
            Assert.Null(result.Result);
            Assert.Contains("Request failed with status code", result.Error);
        }

        [Fact]
        public async Task SearchAllByArgumentAsync_ShouldReturnPaginatedTips_WhenRequestIsSuccessful()
        {
            var tipsCriteria = new TipsDTO { Content = "Hydrate" };
            var expectedData = new PaginatedResult<IEnumerable<TipsDTO?>>
            {
                Items = new List<IEnumerable<TipsDTO?>>
                {
                    new List<TipsDTO?>
                    {
                        new TipsDTO { TipId = Guid.NewGuid(), Content = "Stay hydrated", TipsTypeId = 1 }
                    }
                },
                TotalCount = 1
            };

            SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(expectedData));

            var result = await _tipsService.SearchAllByArgumentAsync(tipsCriteria, null, null);

            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.Equal(1, result.Result.TotalCount);
            Assert.Equal(string.Empty, result.Error);
        }

        [Fact]
        public async Task GetByArgumentAsync_ShouldReturnTip_WhenRequestIsSuccessful()
        {
            var tipsCriteria = new TipsDTO { TipId = Guid.NewGuid() };
            var expectedTip = new TipsDTO { TipId = tipsCriteria.TipId, Content = "Take breaks", TipsTypeId = 3 };

            SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(expectedTip));

            var result = await _tipsService.GetByArgumentAsync(tipsCriteria);

            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.Equal("Take breaks", result.Result.Content);
            Assert.Equal(string.Empty, result.Error);
        }

        [Fact]
        public async Task AddAsync_ShouldReturnSuccess_WhenRequestIsSuccessful()
        {
            var newTip = new TipsDTO { TipId = Guid.NewGuid(), Content = "Exercise daily", TipsTypeId = 1 };

            SetupHttpResponse(HttpStatusCode.Created);

            var result = await _tipsService.AddAsync(newTip);

            Assert.NotNull(result);
            Assert.True(result.Result);
            Assert.Equal(string.Empty, result.Error);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenRequestIsSuccessful()
        {
            var updatedTip = new TipsDTO { TipId = Guid.NewGuid(), Content = "Drink water", TipsTypeId = 1 };

            SetupHttpResponse(HttpStatusCode.OK);

            var result = await _tipsService.UpdateAsync(updatedTip);

            Assert.NotNull(result);
            Assert.True(result.Result);
            Assert.Equal(string.Empty, result.Error);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenRequestIsSuccessful()
        {
            var tipId = Guid.NewGuid();

            SetupHttpResponse(HttpStatusCode.OK);

            var result = await _tipsService.DeleteAsync(tipId);

            Assert.NotNull(result);
            Assert.True(result.Result);
            Assert.Equal(string.Empty, result.Error);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenRequestFails()
        {
            var tipId = Guid.NewGuid();

            SetupHttpResponse(HttpStatusCode.BadRequest, "Bad Request");

            var result = await _tipsService.DeleteAsync(tipId);

            Assert.NotNull(result);
            Assert.False(result.Result);
            Assert.Contains("Request failed with status code", result.Error);
        }
    }
}
