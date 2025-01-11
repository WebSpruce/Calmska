namespace Calmska.Tests.ServicesTests
{
    public class MoodHistoryServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly MoodHistoryService _moodHistoryService;
        public MoodHistoryServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _moodHistoryService = new MoodHistoryService(_httpClient);
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
        public async Task GetAllAsync_ShouldReturnMoodHistoryList_WhenRequestIsSuccessful()
        {
            var expectedData = new List<MoodHistoryDTO?>
            {
                new MoodHistoryDTO { MoodHistoryId = Guid.NewGuid(), UserId = Guid.NewGuid(), MoodId = Guid.NewGuid() }
            };
            SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(expectedData));

            var result = await _moodHistoryService.GetAllAsync(null, null);

            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.Single(result.Result);
            Assert.Equal(string.Empty, result.Error);
        }
        [Fact]
        public async Task SearchAllByArgumentAsync_ShouldReturnPaginatedMoodHistory_WhenRequestIsSuccessful()
        {
            var moodHistoryCriteria = new MoodHistoryDTO { UserId = Guid.NewGuid() };
            var expectedData = new PaginatedResult<IEnumerable<MoodHistoryDTO?>>
            {
                Items = new List<IEnumerable<MoodHistoryDTO?>>
                {
                    new List<MoodHistoryDTO?>
                    {
                        new MoodHistoryDTO { MoodHistoryId = Guid.NewGuid(), UserId = moodHistoryCriteria.UserId, MoodId = Guid.NewGuid() }
                    }
                },
                TotalCount = 1
            };
            SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(expectedData));

            var result = await _moodHistoryService.SearchAllByArgumentAsync(moodHistoryCriteria, null, null);

            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.Equal(1, result.Result.TotalCount);
            Assert.Equal(string.Empty, result.Error);
        }
        [Fact]
        public async Task GetByArgumentAsync_ShouldReturnSingleMoodHistory_WhenRequestIsSuccessful()
        {
            var moodHistoryCriteria = new MoodHistoryDTO { MoodHistoryId = Guid.NewGuid() };
            var expectedData = new MoodHistoryDTO { MoodHistoryId = moodHistoryCriteria.MoodHistoryId, UserId = Guid.NewGuid(), MoodId = Guid.NewGuid() };
            SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(expectedData));

            var result = await _moodHistoryService.GetByArgumentAsync(moodHistoryCriteria);

            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.Equal(moodHistoryCriteria.MoodHistoryId, result.Result?.MoodHistoryId);
            Assert.Equal(string.Empty, result.Error);
        }
        [Fact]
        public async Task AddAsync_ShouldReturnSuccess_WhenRequestIsSuccessful()
        {
            var newMoodHistory = new MoodHistoryDTO { MoodHistoryId = Guid.NewGuid(), UserId = Guid.NewGuid(), MoodId = Guid.NewGuid() };
            SetupHttpResponse(HttpStatusCode.Created, string.Empty);

            var result = await _moodHistoryService.AddAsync(newMoodHistory);

            Assert.NotNull(result);
            Assert.True(result.Result);
            Assert.Equal(string.Empty, result.Error);
        }
        [Fact]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenRequestIsSuccessful()
        {
            var updatedMoodHistory = new MoodHistoryDTO { MoodHistoryId = Guid.NewGuid(), UserId = Guid.NewGuid(), MoodId = Guid.NewGuid() };
            SetupHttpResponse(HttpStatusCode.OK, string.Empty);

            var result = await _moodHistoryService.UpdateAsync(updatedMoodHistory);

            Assert.NotNull(result);
            Assert.True(result.Result);
            Assert.Equal(string.Empty, result.Error);
        }
        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenRequestIsSuccessful()
        {
            var moodHistoryId = Guid.NewGuid();
            SetupHttpResponse(HttpStatusCode.OK, string.Empty);

            var result = await _moodHistoryService.DeleteAsync(moodHistoryId);

            Assert.NotNull(result);
            Assert.True(result.Result);
            Assert.Equal(string.Empty, result.Error);
        }

    }
}
