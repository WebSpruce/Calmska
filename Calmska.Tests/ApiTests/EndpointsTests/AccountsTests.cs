using Calmska.Api.Endpoints;
using Calmska.Application.Abstractions;
using Calmska.Application.DTO;
using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Infrastructure.Persistence;
using Calmska.Infrastructure.Persistence.Models;
using Calmska.Infrastructure.Persistence.Security;
using Calmska.Tests.ApiTests.Infrastructure;
using Firebase.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Calmska.Tests.ApiTests.EndpointsTests
{
    public class AccountsTests : IClassFixture<CalmskaApiFactory>
    {
        private readonly CalmskaApiFactory _factory;
        private readonly IPasswordHasher _passwordHasher;
        private readonly HttpClient _client;
        private readonly Mock<IFirebaseAuthClient> _mockFirebase;
        public AccountsTests(CalmskaApiFactory factory)
        {
            _factory = factory;
            _passwordHasher = new PasswordHasher();
            _mockFirebase = new Mock<IFirebaseAuthClient>();
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // remove the original IFirebaseAuthClient instance
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IFirebaseAuthClient));

                    if (descriptor != null)
                        services.Remove(descriptor);
                    
                    services.AddSingleton(_mockFirebase.Object);
                });
            }).CreateClient();
        }
        [Fact]
        public async Task GetAllAccounts_ShouldReturnOk_WhenAccountsExist()
        {
            var id = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            await SeedDocumentAsync(id);
            await SeedDocumentAsync(id2);
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/accounts";

            using var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadFromJsonAsync<PaginatedResult<Account>>();
            content.Should().NotBeNull();
            content.TotalCount.Should().BeGreaterThan(0);
        }
        [Fact]
        public async Task GetAccountBySearch_ShouldReturnNotFound_WhenAccountDoesNotExist()
        {
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/accounts/search?UserId=00000000-0000-0000-0000-000000000000";

            using var response = await _client.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        [Fact]
        public async Task Login_ShouldReturnOk_WhenAccountExist()
        {
            var id = Guid.NewGuid();
            var email = "test@test.com";
            var pass = _passwordHasher.SetHash("mypass");
            await SeedDocumentAsync(id, email: email, pass: pass.Hash);
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/accounts/login";

            var loginDto = new LoginDTO
            {
                Email = email,
                Password = "mypass"
            };
            using var response = await _client.PostAsJsonAsync(endpoint, loginDto);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task AddAccount_ShouldReturnCreated_WhenAccountIsValid()
        {
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/accounts";
            var account = new Account
            {
                UserId = Guid.Parse("e14fa9ac-1cd3-4d2e-8e39-6217d8cb1ded"), 
                UserName = "TestUser", 
                Email = "test@test.com", 
                PasswordHashed = "mypass"
            };

            using var response = await _client.PostAsJsonAsync(endpoint, account);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var location = response.Headers.Location?.ToString();
            location.Should().Contain(account.UserId.ToString());
            
            var saved = await QuerySender.QueryDbAsync(_factory,db => db.Accounts
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.UserName == "TestUser" && x.Email == "test@test.com"));

            saved.Should().NotBeNull();
            saved.UserName.Should().Be("TestUser");
        }
        [Fact]
        public async Task UpdateAccount_ShouldReturnOk_WhenAccountIsUpdated()
        {
            var id = Guid.NewGuid();
            await SeedDocumentAsync(id);
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/accounts";
            var account = new
            {
                UserId = id,
                UserName = "TestUser",
                Email = "test2@test.com",
                PasswordHashed = "newpassword"
            };

            using var response = await _client.PutAsJsonAsync(endpoint, account);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var updated = await QuerySender.QueryDbAsync(_factory,db => db.Accounts
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.UserId == id));
            
            updated.Should().NotBeNull();
            updated.UserName.Should().Be("TestUser");
            updated.Email.Should().Be("test2@test.com");
        }
        [Fact]
        public async Task DeleteAccount_ShouldReturnOk_WhenAccountExists()
        {
            var id = Guid.Parse("8b9a16b8-bf0d-4abf-9891-1da6f44030a4");
            await SeedDocumentAsync(id);
            string endpoint = $"/api/{ApiRoutes.ApiVersionString}/accounts/{id}";

            using var response = await _client.DeleteAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var deleted = await QuerySender.QueryDbAsync(_factory, db => db.Accounts
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.UserId == id));
            deleted.Should().BeNull();
        }
        
        private async Task<AccountDocument> SeedDocumentAsync(
            Guid? userId = null,
            string email = "test@test.com",
            string userName = "test",
            string pass = "test_pass")
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CalmskaDbContext>();

            var account = new AccountDocument()
            {
                UserId = userId ?? Guid.NewGuid(),
                Email = email,
                UserName = userName,
                PasswordHashed = pass
            };

            await db.Accounts.AddAsync(account);
            await db.SaveChangesAsync();

            return account;
        }
    }
}
