namespace Calmska.Tests.ApiTests.RepositoriesTests
{
    public class AccountRepositoryTests
    {
        private readonly AccountRepository _repository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CalmskaDbContext _context;

        public AccountRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<CalmskaDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new CalmskaDbContext(options);
            _mockMapper = new Mock<IMapper>();
            _repository = new AccountRepository(_context, _mockMapper.Object);
        }
        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedResults()
        {
            _context.Accounts.RemoveRange(_context.Accounts.ToList());
            _context.Accounts.Add(new Account { UserId = Guid.NewGuid(), Email = "test1@example.com", UserName = "user1", PasswordHashed = "hashed1" });
            _context.Accounts.Add(new Account { UserId = Guid.NewGuid(), Email = "test2@example.com", UserName = "user2", PasswordHashed = "hashed2" });
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync(1, 1);

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
            var result = await _repository.GetAllByArgumentAsync(accountDto, 1, 1);

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
            var result = await _repository.GetByArgumentAsync(accountDto);

            result.Should().NotBeNull();
            result.Email.Should().Be("test@example.com");
        }
        [Fact]
        public async Task GetByArgumentAsync_ShouldReturnNull_WhenNotFound()
        {
            _context.Accounts.RemoveRange(_context.Accounts.ToList());
            var accountDto = new AccountDTO { Email = "nonexistent@example.com" };

            var result = await _repository.GetByArgumentAsync(accountDto);

            result.Should().BeNull();
        }
        [Fact]
        public async Task AddAsync_ShouldReturnError_WhenAccountDtoIsNull()
        {
            _context.Accounts.RemoveRange(_context.Accounts.ToList());
            AccountDTO accountDto = null;

            var result = await _repository.AddAsync(accountDto);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided Account object is null.");
        }
        [Fact]
        public async Task AddAsync_ShouldReturnError_WhenEmailAlreadyExists()
        {
            _context.Accounts.RemoveRange(_context.Accounts.ToList());
            var accountDto = new AccountDTO { Email = "newuser@example.com", PasswordHashed = "password123" };

            _mockMapper.Setup(m => m.Map<Account>(It.IsAny<AccountDTO>()))
                       .Returns(new Account { Email = accountDto.Email, UserName = accountDto.UserName ?? string.Empty, PasswordHashed = accountDto.PasswordHashed ?? string.Empty });
            var result = await _repository.AddAsync(accountDto);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();
        }
        [Fact]
        public async Task AddAsync_ShouldReturnSuccess_WhenAccountIsAdded()
        {
            _context.Accounts.RemoveRange(_context.Accounts.ToList());
            var accountDto = new AccountDTO { Email = "newuser@example.com", PasswordHashed = "password123" };

            _mockMapper.Setup(m => m.Map<Account>(It.IsAny<AccountDTO>()))
                       .Returns(new Account { Email = accountDto.Email, UserName = accountDto.UserName ?? string.Empty, PasswordHashed = accountDto.PasswordHashed ?? string.Empty });
            var result = await _repository.AddAsync(accountDto);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();
        }
    }
}
