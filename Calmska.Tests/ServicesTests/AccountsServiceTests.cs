namespace Calmska.Tests.ServicesTests
{
    public class AccountsServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly AccountsService _accountsService;
        public AccountsServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _accountsService = new AccountsService(_httpClient);
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
        public async Task GetAllAsync_ShouldReturnAccounts_WhenRequestIsSuccessful()
        {
            var expectedData = new List<AccountDTO?>
            {
                new AccountDTO { UserId = Guid.NewGuid(), UserName = "User1", Email = "user1@example.com" },
                new AccountDTO { UserId = Guid.NewGuid(), UserName = "User2", Email = "user2@example.com" }
            };

            SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(expectedData));

            var result = await _accountsService.GetAllAsync(null, null);

            Assert.NotNull(result);
            Assert.True(result.Result != null && result.Result.Any());
            Assert.Equal(string.Empty, result.Error);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnError_WhenRequestFails()
        {
            SetupHttpResponse(HttpStatusCode.BadRequest, "Bad Request");
  
            var result = await _accountsService.GetAllAsync(null, null);
 
            Assert.NotNull(result);
            Assert.Null(result.Result);
            Assert.Contains("Request failed with status code", result.Error);
        }

        [Fact]
        public async Task SearchAllByArgumentAsync_ShouldReturnAccounts_WhenRequestIsSuccessful()
        {
            var accountCriteria = new AccountDTO { UserName = "User1" };
            var expectedData = new PaginatedResult<IEnumerable<AccountDTO?>>
            {
                Items = new List<IEnumerable<AccountDTO?>>
                {
                    new List<AccountDTO?>
                    {
                        new AccountDTO { UserId = Guid.NewGuid(), UserName = "User1", Email = "user1@example.com" }
                    }
                },
                TotalCount = 1
            };

            SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(expectedData));

            var result = await _accountsService.SearchAllByArgumentAsync(accountCriteria, null, null);

            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.Equal(1, result.Result.TotalCount);
            Assert.Equal(string.Empty, result.Error);
        }


        [Fact]
        public async Task GetByArgumentAsync_ShouldReturnAccount_WhenRequestIsSuccessful()
        {  
            var accountCriteria = new AccountDTO { UserId = Guid.NewGuid() };
            var expectedAccount = new AccountDTO { UserId = accountCriteria.UserId, UserName = "User1", Email = "user1@example.com" };

            SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(expectedAccount));

            var result = await _accountsService.GetByArgumentAsync(accountCriteria);

            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.Equal("User1", result.Result.UserName);
            Assert.Equal(string.Empty, result.Error);
        }
        [Fact]
        public async Task LoginAsync_ShouldReturnTrue_WhenRequestIsSuccessful()
        {  
            var account = new AccountDTO { UserId = Guid.Empty, UserName = "", Email = "test@test.com", PasswordHashed = "mypass" };

            SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(true));

            var result = await _accountsService.LoginAsync(account);

            Assert.NotNull(result);
            Assert.True(result.Result);
            Assert.Equal(string.Empty, result.Error);
        }

        [Fact]
        public async Task AddAsync_ShouldReturnSuccess_WhenRequestIsSuccessful()
        {
            var newAccount = new AccountDTO { UserId = Guid.NewGuid(), UserName = "NewUser", Email = "newuser@example.com" };

            SetupHttpResponse(HttpStatusCode.Created);

            var result = await _accountsService.AddAsync(newAccount);

            Assert.NotNull(result);
            Assert.True(result.Result);
            Assert.Equal(string.Empty, result.Error);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenRequestIsSuccessful()
        {
            var updatedAccount = new AccountDTO { UserId = Guid.NewGuid(), UserName = "UpdatedUser", Email = "updated@example.com" };

            SetupHttpResponse(HttpStatusCode.OK);

            var result = await _accountsService.UpdateAsync(updatedAccount);

            Assert.NotNull(result);
            Assert.True(result.Result);
            Assert.Equal(string.Empty, result.Error);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenRequestIsSuccessful()
        {
            var accountId = Guid.NewGuid();

            SetupHttpResponse(HttpStatusCode.OK);

            var result = await _accountsService.DeleteAsync(accountId);

            Assert.NotNull(result);
            Assert.True(result.Result);
            Assert.Equal(string.Empty, result.Error);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenRequestFails()
        {
            var accountId = Guid.NewGuid();

            SetupHttpResponse(HttpStatusCode.BadRequest, "Bad Request");

            var result = await _accountsService.DeleteAsync(accountId);

            Assert.NotNull(result);
            Assert.False(result.Result);
            Assert.Contains("Request failed with status code", result.Error);
        }
    }
}