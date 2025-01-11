namespace Calmska.Tests.ServicesTests
{
    public class HttpClientHelperTests
    {
        private Mock<HttpMessageHandler> CreateMockHandler(HttpResponseMessage responseMessage)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            return handlerMock;
        }


        [Fact]
        public async Task GetAsync_ShouldReturnResult_WhenRequestIsSuccessful()
        {
            var endpoint = "/test";
            var expectedData = new TestData { Name = "Test", Value = 42 };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(expectedData), Encoding.UTF8, "application/json")
            };

            var handlerMock = CreateMockHandler(responseMessage);
            var client = new HttpClient(handlerMock.Object);

            var result = await HttpClientHelper.GetAsync<TestData>(client, endpoint);

            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Error);
            Assert.Equal("Test", result.Result?.Name);
            Assert.Equal(42, result.Result?.Value);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnError_WhenRequestFails()
        {
            var endpoint = "/test";
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Bad Request")
            };

            var handlerMock = CreateMockHandler(responseMessage);
            var client = new HttpClient(handlerMock.Object);

            var result = await HttpClientHelper.GetAsync<dynamic>(client, endpoint);

            Assert.NotNull(result);
            Assert.Null(result.Result);
            Assert.Contains("Request failed with status code", result.Error);
        }

        [Fact]
        public async Task PostAsync_ShouldReturnSuccess_WhenRequestIsSuccessful()
        {
            var endpoint = "/test";
            var requestData = new TestData { Name = "Test", Value = 42 };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            var handlerMock = CreateMockHandler(responseMessage);
            var client = new HttpClient(handlerMock.Object);

            var result = await HttpClientHelper.PostAsync(client, endpoint, requestData);

            Assert.NotNull(result);
            Assert.True(result.Result);
            Assert.Equal(string.Empty, result.Error);
        }


        [Fact]
        public async Task PostAsync_ShouldReturnError_WhenRequestFails()
        {
            var endpoint = "/test";
            var requestData = new TestData { Name = "Test", Value = 42 };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Bad Request")
            };

            var handlerMock = CreateMockHandler(responseMessage);
            var client = new HttpClient(handlerMock.Object);

            var result = await HttpClientHelper.PostAsync(client, endpoint, requestData);

            Assert.NotNull(result);
            Assert.False(result.Result);
            Assert.Contains("Request failed with status code", result.Error);
        }


        [Fact]
        public async Task PutAsync_ShouldReturnSuccess_WhenRequestIsSuccessful()
        {
            var endpoint = "/test";
            var requestData = new TestData { Name = "UpdatedTest", Value = 99 };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            var handlerMock = CreateMockHandler(responseMessage);
            var client = new HttpClient(handlerMock.Object);

            var result = await HttpClientHelper.PutAsync(client, endpoint, requestData);

            Assert.NotNull(result);
            Assert.True(result.Result);
            Assert.Equal(string.Empty, result.Error);
        }


        [Fact]
        public async Task PutAsync_ShouldReturnError_WhenRequestFails()
        {
            var endpoint = "/test";
            var requestData = new TestData { Name = "UpdatedTest", Value = 99 };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Bad Request")
            };

            var handlerMock = CreateMockHandler(responseMessage);
            var client = new HttpClient(handlerMock.Object);

            var result = await HttpClientHelper.PutAsync(client, endpoint, requestData);

            Assert.NotNull(result);
            Assert.False(result.Result);
            Assert.Contains("Request failed with status code", result.Error);
        }


        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenRequestIsSuccessful()
        {
            var endpoint = "/test";
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            var handlerMock = CreateMockHandler(responseMessage);
            var client = new HttpClient(handlerMock.Object);

            var result = await HttpClientHelper.DeleteAsync(client, endpoint);

            Assert.NotNull(result);
            Assert.True(result.Result);
            Assert.Equal(string.Empty, result.Error);
        }


        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenRequestFails()
        {
            var endpoint = "/test";
            var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("Not Found")
            };

            var handlerMock = CreateMockHandler(responseMessage);
            var client = new HttpClient(handlerMock.Object);

            var result = await HttpClientHelper.DeleteAsync(client, endpoint);

            Assert.NotNull(result);
            Assert.False(result.Result);
            Assert.Contains("Request failed with status code", result.Error);
        }

    }
    internal class TestData
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}
