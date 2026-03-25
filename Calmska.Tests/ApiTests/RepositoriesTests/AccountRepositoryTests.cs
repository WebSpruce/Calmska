using Calmska.Application.DTO;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Infrastructure.Persistence;
using Calmska.Infrastructure.Persistence.Models;
using Calmska.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Logging.Abstractions;

namespace Calmska.Tests.ApiTests.RepositoriesTests
{
    public class AccountRepositoryTests
    {
        private readonly AccountRepository _repository;
        private readonly CalmskaDbContext _context;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly IMapper _mapper;
        
        public AccountRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<CalmskaDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
        
            _context = new CalmskaDbContext(options);
            var mapperConfig = new MapperConfiguration(cfg =>
                cfg.AddProfile<Infrastructure.Mapping.MapperProfiles>(), new NullLoggerFactory());
            var mapper = mapperConfig.CreateMapper();
            _mapper = mapper;
            _repository = new AccountRepository(_context, mapper);
            _cancellationTokenSource = new CancellationTokenSource();
        }
        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedResults()
        {
            _context.Accounts.RemoveRange(_context.Accounts.ToList());
            _context.Accounts.Add(new AccountDocument { UserId = Guid.NewGuid(), Email = "test1@example.com", UserName = "user1", PasswordHashed = "hashed1" });
            _context.Accounts.Add(new AccountDocument { UserId = Guid.NewGuid(), Email = "test2@example.com", UserName = "user2", PasswordHashed = "hashed2" });
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
            _context.Accounts.Add(new AccountDocument { UserId = Guid.NewGuid(), Email = "test1@example.com", UserName = "user1", PasswordHashed = "hashed1" });
            _context.Accounts.Add(new AccountDocument { UserId = Guid.NewGuid(), Email = "test2@example.com", UserName = "user2", PasswordHashed = "hashed2" });
            await _context.SaveChangesAsync();
        
            var accountDto = new AccountFilter(null, null, "test1@example.com", null);
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
            var account = new AccountDocument { UserId = Guid.NewGuid(), Email = "test@example.com", UserName = "user1", PasswordHashed = "hashed1" };
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        
            var accountDto = new AccountFilter(null, null, "test@example.com", null);
            var result = await _repository.GetByArgumentAsync(accountDto, _cancellationTokenSource.Token);
        
            result.Should().NotBeNull();
            result.Email.Should().Be("test@example.com");
        }
        [Fact]
        public async Task LoginAsync_ShouldTrue_WhenCredentialAreCorrect()
        {
            _context.Accounts.RemoveRange(_context.Accounts.ToList());
            var account = new AccountDocument { UserId = Guid.NewGuid(), Email = "test@test.com", UserName = "user1", PasswordHashed = "mypass" };
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        
            var result = await _repository.GetByArgumentAsync(new AccountFilter(null, null, "test@test.com", "mypass"), _cancellationTokenSource.Token);
        
            result.Should().NotBeNull();
        }
        [Fact]
        public async Task GetByArgumentAsync_ShouldReturnNull_WhenNotFound()
        {
            _context.Accounts.RemoveRange(_context.Accounts.ToList());
            var accountDto = new AccountFilter(null, null, "nonexistent@example.com", null);
        
            var result = await _repository.GetByArgumentAsync(accountDto, _cancellationTokenSource.Token);
        
            result.Should().BeNull();
        }
        [Fact]
        public async Task AddAsync_ShouldReturnSuccess_WhenAccountIsAdded()
        {
            _context.Accounts.RemoveRange(_context.Accounts.ToList());
            var account = new Account { UserId = Guid.Empty, UserName = null, Email ="newuser@example.com", PasswordHashed= "password123" };
            
            var result = await _repository.AddAsync(account, _cancellationTokenSource.Token);
        
            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();
        }
        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenAccountNotFound()
        {
            var filter = new AccountFilter(Guid.NewGuid(), "nonexistentuser", "", "");
        
            var result = await _repository.UpdateAsync(filter, _cancellationTokenSource.Token);
        
            result.Result.Should().BeFalse();
            result.Error.Should().Be("Didn't find any account with the provided userId.");
        }
        [Fact]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenAccountIsUpdated()
        {
            _context.Accounts.RemoveRange(_context.Accounts.ToList());
            var existingAccount = new AccountDocument { UserId = Guid.NewGuid(), Email = "test@example.com", UserName = "existinguser", PasswordHashed = BCrypt.Net.BCrypt.HashPassword("password123") };
            _context.Accounts.Add(existingAccount);
            await _context.SaveChangesAsync();
        
            var filter = new AccountFilter(existingAccount.UserId, "updateduser", "updated@example.com", "newpassword");
            
            var result = await _repository.UpdateAsync(filter, _cancellationTokenSource.Token);
        
            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();
        
            var updatedAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == existingAccount.UserId);
            updatedAccount.Should().NotBeNull();
            updatedAccount!.UserName.Should().Be(filter.UserName);
            updatedAccount.Email.Should().Be(filter.Email);
            BCrypt.Net.BCrypt.Verify(filter.PasswordHashed, updatedAccount.PasswordHashed).Should().BeTrue();
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
            var existingAccount = new AccountDocument { UserId = Guid.NewGuid(), Email = "test@example.com", UserName = "userToDelete", PasswordHashed = "hashedpassword" };
            _context.Accounts.Add(existingAccount);
            await _context.SaveChangesAsync();
        
            var result = await _repository.DeleteAsync((Guid)existingAccount.UserId, _cancellationTokenSource.Token);
        
            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();
        
            var deletedAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == existingAccount.UserId);
            deletedAccount.Should().BeNull();
        }
    }
}
