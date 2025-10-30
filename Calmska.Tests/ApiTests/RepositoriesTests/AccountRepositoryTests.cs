using Firebase.Auth;
using Firebase.Auth.Providers;

namespace Calmska.Tests.ApiTests.RepositoriesTests
{
    public class AccountRepositoryTests
    {
        private readonly AccountRepository _repository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CalmskaDbContext _context;
        private readonly Mock<IFirebaseAuthClient> _authClient;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public AccountRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<CalmskaDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new CalmskaDbContext(options);
            _mockMapper = new Mock<IMapper>();
            _authClient = new Mock<IFirebaseAuthClient>();
            _repository = new AccountRepository(_context, _mockMapper.Object, _authClient.Object);
            _cancellationTokenSource = new CancellationTokenSource();
        }
        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedResults()
        {
            _context.Accounts.RemoveRange(_context.Accounts.ToList());
            _context.Accounts.Add(new Account { UserId = Guid.NewGuid(), Email = "test1@example.com", UserName = "user1", PasswordHashed = "hashed1" });
            _context.Accounts.Add(new Account { UserId = Guid.NewGuid(), Email = "test2@example.com", UserName = "user2", PasswordHashed = "hashed2" });
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync(1, 1, _cancellationTokenSource.Token);

            result.Items.Should().HaveCount(1);
            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(1);
            result.TotalCount.Should().Be(2);
        }
        [Fact]
        public async Task GetAllByArgumentAsync_ShouldReturnFilteredPaginatedResults()
        {
            _context.Accounts.RemoveRange(_context.Accounts.ToList());
            _context.Accounts.Add(new Account { UserId = Guid.NewGuid(), Email = "test1@example.com", UserName = "user1", PasswordHashed = "hashed1" });
            _context.Accounts.Add(new Account { UserId = Guid.NewGuid(), Email = "test2@example.com", UserName = "user2", PasswordHashed = "hashed2" });
            await _context.SaveChangesAsync();

            var accountDto = new AccountDTO { Email = "test1@example.com" };
            var result = await _repository.GetAllByArgumentAsync(accountDto, 1, 1, _cancellationTokenSource.Token);

            result.Items.Should().HaveCount(1);
            result.Items.First().Email.Should().Be("test1@example.com");
            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(1);
            result.TotalCount.Should().Be(1);
        }
        [Fact]
        public async Task GetByArgumentAsync_ShouldReturnAccount_WhenFound()
        {
            _context.Accounts.RemoveRange(_context.Accounts.ToList());
            var account = new Account { UserId = Guid.NewGuid(), Email = "test@example.com", UserName = "user1", PasswordHashed = "hashed1" };
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            var accountDto = new AccountDTO { Email = "test@example.com" };
            var result = await _repository.GetByArgumentAsync(accountDto, _cancellationTokenSource.Token);

            result.Should().NotBeNull();
            result.Email.Should().Be("test@example.com");
        }
        [Fact]
        public async Task LoginAsync_ShouldTrue_WhenCredentialAreCorrect()
        {
            _context.Accounts.RemoveRange(_context.Accounts.ToList());
            var account = new Account { UserId = Guid.NewGuid(), Email = "test@test.com", UserName = "user1", PasswordHashed = "$2a$10$p4BKLzY7oYIBfE9Bghj6gerIeaNeWjjIkk2r3kL5xrBncTriO.Mz." };
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            var result = await _repository.LoginAsync(new AccountDTO { Email = "test@test.com", PasswordHashed = "mypass" }, _cancellationTokenSource.Token);

            result.Should().BeTrue();
        }
        [Fact]
        public async Task GetByArgumentAsync_ShouldReturnNull_WhenNotFound()
        {
            _context.Accounts.RemoveRange(_context.Accounts.ToList());
            var accountDto = new AccountDTO { Email = "nonexistent@example.com" };

            var result = await _repository.GetByArgumentAsync(accountDto, _cancellationTokenSource.Token);

            result.Should().BeNull();
        }
        [Fact]
        public async Task AddAsync_ShouldReturnError_WhenAccountDtoIsNull()
        {
            _context.Accounts.RemoveRange(_context.Accounts.ToList());
            AccountDTO accountDto = null;

            var result = await _repository.AddAsync(accountDto, _cancellationTokenSource.Token);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided Account object is null.");
        }
        [Fact]
        public async Task AddAsync_ShouldReturnError_WhenEmailAlreadyExists()
        {
            _context.Accounts.RemoveRange(_context.Accounts.ToList());
            var existingAccount = new Account{ UserId = Guid.NewGuid(), Email = "test@example.com", UserName = "existinguser", PasswordHashed = "hashedpassword" };
            _context.Accounts.Add(existingAccount);
            await _context.SaveChangesAsync();

            var accountDto = new AccountDTO{ Email = "test@example.com", UserName = "newuser", PasswordHashed = "newpassword", UserId = null};

            var result = await _repository.AddAsync(accountDto, _cancellationTokenSource.Token);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("The user with provided email exists.");
        }
        [Fact]
        public async Task AddAsync_ShouldReturnSuccess_WhenAccountIsAdded()
        {
            _context.Accounts.RemoveRange(_context.Accounts.ToList());
            var accountDto = new AccountDTO { Email = "newuser@example.com", PasswordHashed = "password123" };

            _mockMapper.Setup(m => m.Map<Account>(It.IsAny<AccountDTO>()))
                       .Returns(new Account { Email = accountDto.Email, UserName = accountDto.UserName ?? string.Empty, PasswordHashed = accountDto.PasswordHashed ?? string.Empty });
            var result = await _repository.AddAsync(accountDto, _cancellationTokenSource.Token);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();
        }
        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenAccountDtoIsNull()
        {
            AccountDTO accountDto = null;

            var result = await _repository.UpdateAsync(accountDto, _cancellationTokenSource.Token);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided Account object is null.");
        }
        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenAccountNotFound()
        {
            var accountDto = new AccountDTO { UserId = Guid.NewGuid(), UserName = "nonexistentuser" };

            var result = await _repository.UpdateAsync(accountDto, _cancellationTokenSource.Token);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("Didn't find any account with the provided userId.");
        }
        [Fact]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenAccountIsUpdated()
        {
            _context.Accounts.RemoveRange(_context.Accounts.ToList());
            var existingAccount = new Account { UserId = Guid.NewGuid(), Email = "test@example.com", UserName = "existinguser", PasswordHashed = BCrypt.Net.BCrypt.HashPassword("password123") };
            _context.Accounts.Add(existingAccount);
            await _context.SaveChangesAsync();

            var accountDto = new AccountDTO { UserId = existingAccount.UserId, UserName = "updateduser", Email = "updated@example.com", PasswordHashed = "newpassword" };

            var result = await _repository.UpdateAsync(accountDto, _cancellationTokenSource.Token);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();

            var updatedAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == existingAccount.UserId);
            updatedAccount.Should().NotBeNull();
            updatedAccount!.UserName.Should().Be(accountDto.UserName);
            updatedAccount.Email.Should().Be(accountDto.Email);
            BCrypt.Net.BCrypt.Verify(accountDto.PasswordHashed, updatedAccount.PasswordHashed).Should().BeTrue();
        }
        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenAccountIdIsEmpty()
        {
            var accountId = Guid.Empty;

            var result = await _repository.DeleteAsync(accountId, _cancellationTokenSource.Token);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided AccountId is null.");
        }
        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenAccountNotFound()
        {
            var accountId = Guid.NewGuid();

            var result = await _repository.DeleteAsync(accountId, _cancellationTokenSource.Token);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("There is no settings with provided id.");
        }
        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenAccountIsDeleted()
        {
            var existingAccount = new Account { UserId = Guid.NewGuid(), Email = "test@example.com", UserName = "userToDelete", PasswordHashed = "hashedpassword" };
            _context.Accounts.Add(existingAccount);
            await _context.SaveChangesAsync();

            var result = await _repository.DeleteAsync(existingAccount.UserId, _cancellationTokenSource.Token);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();

            var deletedAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == existingAccount.UserId);
            deletedAccount.Should().BeNull();
        }
    }
}
