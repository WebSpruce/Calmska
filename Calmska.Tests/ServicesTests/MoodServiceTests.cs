using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calmska.Tests.ServicesTests
{
    public class MoodServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly MoodService _moodService;

        public MoodServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _moodService = new MoodService(_httpClient);
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
        public async Task GetAllAsync_ShouldReturnMoods_WhenRequestIsSuccessful()
        {
            var expectedData = new List<MoodDTO?>
        {
            new MoodDTO { MoodId = Guid.NewGuid(), MoodName = "Happy", Type = "Positive" },
            new MoodDTO { MoodId = Guid.NewGuid(), MoodName = "Sad", Type = "Negative" }
        };

            SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(expectedData));

            var result = await _moodService.GetAllAsync(null, null);

            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.True(result.Result.Any());
            Assert.Equal(string.Empty, result.Error);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnError_WhenRequestFails()
        {
            SetupHttpResponse(HttpStatusCode.BadRequest, "Bad Request");

            var result = await _moodService.GetAllAsync(null, null);

            Assert.NotNull(result);
            Assert.Null(result.Result);
            Assert.Contains("Request failed with status code", result.Error);
        }

        [Fact]
        public async Task SearchAllByArgumentAsync_ShouldReturnPaginatedMoods_WhenRequestIsSuccessful()
        {
            var moodCriteria = new MoodDTO { MoodName = "Happy" };
            var expectedData = new PaginatedResult<IEnumerable<MoodDTO?>>
            {
                Items = new List<IEnumerable<MoodDTO?>>
                {
                    new List<MoodDTO?>
                    {
                        new MoodDTO { MoodId = Guid.NewGuid(), MoodName = "Happy", Type = "Positive" }
                    }
                },
                TotalCount = 1
            };

            SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(expectedData));

            var result = await _moodService.SearchAllByArgumentAsync(moodCriteria, null, null);

            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.Equal(1, result.Result.TotalCount);
            Assert.Equal(string.Empty, result.Error);
        }


        [Fact]
        public async Task GetByArgumentAsync_ShouldReturnMood_WhenRequestIsSuccessful()
        {
            var moodCriteria = new MoodDTO { MoodId = Guid.NewGuid() };
            var expectedMood = new MoodDTO { MoodId = moodCriteria.MoodId, MoodName = "Calm", Type = "Neutral" };

            SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(expectedMood));

            var result = await _moodService.GetByArgumentAsync(moodCriteria);

            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.Equal("Calm", result.Result.MoodName);
            Assert.Equal(string.Empty, result.Error);
        }

        [Fact]
        public async Task AddAsync_ShouldReturnSuccess_WhenRequestIsSuccessful()
        {
            var newMood = new MoodDTO { MoodId = Guid.NewGuid(), MoodName = "Excited", Type = "Positive" };

            SetupHttpResponse(HttpStatusCode.Created);

            var result = await _moodService.AddAsync(newMood);

            Assert.NotNull(result);
            Assert.True(result.Result);
            Assert.Equal(string.Empty, result.Error);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenRequestIsSuccessful()
        {
            var updatedMood = new MoodDTO { MoodId = Guid.NewGuid(), MoodName = "UpdatedMood", Type = "Neutral" };

            SetupHttpResponse(HttpStatusCode.OK);

            var result = await _moodService.UpdateAsync(updatedMood);

            Assert.NotNull(result);
            Assert.True(result.Result);
            Assert.Equal(string.Empty, result.Error);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenRequestIsSuccessful()
        {
            var moodId = Guid.NewGuid();

            SetupHttpResponse(HttpStatusCode.OK);

            var result = await _moodService.DeleteAsync(moodId);

            Assert.NotNull(result);
            Assert.True(result.Result);
            Assert.Equal(string.Empty, result.Error);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenRequestFails()
        {
            var moodId = Guid.NewGuid();

            SetupHttpResponse(HttpStatusCode.BadRequest, "Bad Request");

            var result = await _moodService.DeleteAsync(moodId);

            Assert.NotNull(result);
            Assert.False(result.Result);
            Assert.Contains("Request failed with status code", result.Error);
        }
    }
}
