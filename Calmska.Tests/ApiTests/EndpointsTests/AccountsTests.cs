namespace Calmska.Tests.ApiTests.EndpointsTests
{
    public class AccountsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public AccountsTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }
        [Fact]
        public async Task GetAllAccounts_ShouldReturnOk_WhenAccountsExist()
        {
            string endpoint = "/api/v1/accounts";

            var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadFromJsonAsync<PaginatedResult<Account>>();
            content.Should().NotBeNull();
            content.TotalCount.Should().BeGreaterThan(0);
        }
        [Fact]
        public async Task GetAccountBySearch_ShouldReturnNotFound_WhenAccountDoesNotExist()
        {
            string endpoint = "/api/v1/accounts/search?UserId=00000000-0000-0000-0000-000000000000";

            var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        [Fact]
        public async Task AddAccount_ShouldReturnCreated_WhenAccountIsValid()
        {
            string endpoint = "/api/v1/accounts";
            var account = new Account { UserId = Guid.Parse("e14fa9ac-1cd3-4d2e-8e39-6217d8cb1ded"), UserName = "TestUser", Email = "test@test.com", PasswordHashed = "mypass" };

            var response = await _client.PostAsJsonAsync<Account>(endpoint, account);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var location = response.Headers.Location.ToString();
            location.Should().Contain(account.UserId.ToString());
        }
        [Fact]
        public async Task UpdateAccount_ShouldReturnOk_WhenAccountIsUpdated()
        {
            string endpoint = "/api/v1/accounts";
            var account = new
            {
                UserId = Guid.Parse("e14fa9ac-1cd3-4d2e-8e39-6217d8cb1ded"),
                UserName = "TestUser",
                Email = "test@test.com",
                PasswordHashed = "newpassword"
            };

            var response = await _client.PutAsJsonAsync(endpoint, account);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task DeleteAccount_ShouldReturnOk_WhenAccountExists()
        {
            string endpoint = "/api/v1/accounts?accountId=e14fa9ac-1cd3-4d2e-8e39-6217d8cb1ded";

            var response = await _client.DeleteAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
