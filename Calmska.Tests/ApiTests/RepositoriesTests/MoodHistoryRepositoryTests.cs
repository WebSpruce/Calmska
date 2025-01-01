using Calmska.Models.DTO;
using Calmska.Models.Models;
using Microsoft.Extensions.Options;
using System;

namespace Calmska.Tests.ApiTests.RepositoriesTests
{
    public class MoodHistoryRepositoryTests
    {
        private readonly MoodHistoryRepository _repository;
        private readonly Mock<IMapper> _mapper;
        private readonly CalmskaDbContext _context;
        public MoodHistoryRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<CalmskaDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new CalmskaDbContext(options);
            _mapper = new Mock<IMapper>();
            _repository = new MoodHistoryRepository(_context, _mapper.Object);
        }
        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedResults()
        {
            _context.MoodHistoryDb.RemoveRange(_context.MoodHistoryDb.ToList());
            _context.MoodHistoryDb.Add(new MoodHistory { MoodHistoryId = Guid.NewGuid(), MoodId = Guid.NewGuid(), UserId = Guid.NewGuid(), Date = DateTime.UtcNow });
            _context.MoodHistoryDb.Add(new MoodHistory { MoodHistoryId = Guid.NewGuid(), MoodId = Guid.NewGuid(), UserId = Guid.NewGuid(), Date = DateTime.UtcNow });
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
            _context.MoodHistoryDb.RemoveRange(_context.MoodHistoryDb.ToList());
            var guid = Guid.NewGuid();
            _context.MoodHistoryDb.Add(new MoodHistory { MoodHistoryId = guid, MoodId = Guid.NewGuid(), UserId = Guid.NewGuid(), Date = DateTime.UtcNow });
            _context.MoodHistoryDb.Add(new MoodHistory { MoodHistoryId = Guid.NewGuid(), MoodId = Guid.NewGuid(), UserId = Guid.NewGuid(), Date = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            var moodHistoryDto = new MoodHistoryDTO { MoodHistoryId = guid };
            var result = await _repository.GetAllByArgumentAsync(moodHistoryDto, 1, 1);

            result.Items.Should().HaveCount(1);
            result.Items.First().MoodHistoryId.Should().Be(guid);
            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(1);
            result.TotalCount.Should().Be(1);
        }
        [Fact]
        public async Task GetByArgumentAsync_ShouldReturnAccount_WhenFound()
        {
            _context.MoodHistoryDb.RemoveRange(_context.MoodHistoryDb.ToList());
            var guid = Guid.NewGuid();
            var moodHistory = new MoodHistory { MoodHistoryId = guid, MoodId = Guid.NewGuid(), UserId = Guid.NewGuid(), Date = DateTime.UtcNow };
            _context.MoodHistoryDb.Add(moodHistory);
            await _context.SaveChangesAsync();

            var moodHistoryDto = new MoodHistoryDTO { MoodHistoryId = guid };
            var result = await _repository.GetByArgumentAsync(moodHistoryDto);

            result.Should().NotBeNull();
            result.MoodHistoryId.Should().Be(guid);
        }
        [Fact]
        public async Task GetByArgumentAsync_ShouldReturnNull_WhenNotFound()
        {
            _context.MoodHistoryDb.RemoveRange(_context.MoodHistoryDb.ToList());
            var moodHistoryDTO = new MoodHistoryDTO { MoodHistoryId = Guid.NewGuid() };

            var result = await _repository.GetByArgumentAsync(moodHistoryDTO);

            result.Should().BeNull();
        }
        [Fact]
        public async Task AddAsync_ShouldReturnError_WhenAccountDtoIsNull()
        {
            _context.MoodHistoryDb.RemoveRange(_context.MoodHistoryDb.ToList());
            MoodHistoryDTO moodHistoryDTO = null;

            var result = await _repository.AddAsync(moodHistoryDTO);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided MoodHistory object is null.");
        }
        [Fact]
        public async Task AddAsync_ShouldReturnError_WhenEmailAlreadyExists()
        {
            _context.Accounts.RemoveRange(_context.Accounts.ToList());
            var guid = Guid.NewGuid();
            var existingMoodHistory = new MoodHistory { UserId = guid, Date = DateTime.UtcNow, MoodId = Guid.NewGuid(), MoodHistoryId = Guid.NewGuid() };
            _context.MoodHistoryDb.Add(existingMoodHistory);
            await _context.SaveChangesAsync();

            var moodHistoryDto = new MoodHistoryDTO { UserId = guid, Date = DateTime.UtcNow, MoodId = Guid.NewGuid(), MoodHistoryId = Guid.NewGuid() };

            var result = await _repository.AddAsync(moodHistoryDto);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("The moodHistory for provided UserId already exists.");
        }
        [Fact]
        public async Task AddAsync_ShouldReturnSuccess_WhenAccountIsAdded()
        {
            _context.MoodHistoryDb.RemoveRange(_context.MoodHistoryDb.ToList());
            var moodHistoryDto = new MoodHistoryDTO { MoodHistoryId = Guid.NewGuid(), MoodId = Guid.NewGuid(), UserId = Guid.NewGuid(), Date = DateTime.UtcNow };

            _mapper.Setup(m => m.Map<MoodHistory>(It.IsAny<MoodHistoryDTO>()))
                       .Returns(new MoodHistory { MoodHistoryId = moodHistoryDto.MoodHistoryId ?? Guid.Empty, MoodId = moodHistoryDto.MoodId ?? Guid.Empty, UserId = moodHistoryDto.UserId ?? Guid.Empty, Date = moodHistoryDto.Date ?? DateTime.UtcNow });
            var result = await _repository.AddAsync(moodHistoryDto);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();
        }
        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenMoodHistoryDtoIsNull()
        {
            MoodHistoryDTO moodHistoryDTO = null;

            var result = await _repository.UpdateAsync(moodHistoryDTO);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided MoodHistory object is null.");
        }
        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenMoodHistoryNotFound()
        {
            var moodHistoryDTO = new MoodHistoryDTO { MoodHistoryId = Guid.NewGuid(), UserId = Guid.NewGuid(), MoodId = Guid.NewGuid() };

            var result = await _repository.UpdateAsync(moodHistoryDTO);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("Didn't find any account with the provided userId.");
        }
        [Fact]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenMoodHistoryIsUpdated()
        {
            var existingMoodHistory = new MoodHistory { MoodHistoryId = Guid.NewGuid(), UserId = Guid.NewGuid(), MoodId = Guid.NewGuid(), Date = DateTime.UtcNow.AddDays(-1) };
            _context.MoodHistoryDb.Add(existingMoodHistory);
            await _context.SaveChangesAsync();

            var moodHistoryDTO = new MoodHistoryDTO { MoodHistoryId = existingMoodHistory.MoodHistoryId, UserId = Guid.NewGuid(), MoodId = Guid.NewGuid() };

            var result = await _repository.UpdateAsync(moodHistoryDTO);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();

            var updatedMoodHistory = await _context.MoodHistoryDb.FirstOrDefaultAsync(a => a.MoodHistoryId == existingMoodHistory.MoodHistoryId);
            updatedMoodHistory.Should().NotBeNull();
            updatedMoodHistory!.UserId.Should().Be(moodHistoryDTO.UserId.ToString());
            updatedMoodHistory.MoodId.Should().Be(moodHistoryDTO.MoodId.ToString());
            updatedMoodHistory.Date.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }
        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenMoodHistoryIdIsEmpty()
        {
            var moodHistoryId = Guid.Empty;

            var result = await _repository.DeleteAsync(moodHistoryId);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided MoodHistoryId is null.");
        }
        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenMoodHistoryNotFound()
        {
            var moodHistoryId = Guid.NewGuid();

            var result = await _repository.DeleteAsync(moodHistoryId);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("There is no moodHistory with provided id.");
        }
        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenMoodHistoryIsDeleted()
        {
            var existingMoodHistory = new MoodHistory { MoodHistoryId = Guid.NewGuid(), UserId = Guid.NewGuid(), MoodId = Guid.NewGuid(), Date = DateTime.UtcNow };
            _context.MoodHistoryDb.Add(existingMoodHistory);
            await _context.SaveChangesAsync();

            var result = await _repository.DeleteAsync(existingMoodHistory.MoodHistoryId);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();

            var deletedMoodHistory = await _context.MoodHistoryDb.FirstOrDefaultAsync(a => a.MoodHistoryId == existingMoodHistory.MoodHistoryId);
            deletedMoodHistory.Should().BeNull();
        }

    }
}
