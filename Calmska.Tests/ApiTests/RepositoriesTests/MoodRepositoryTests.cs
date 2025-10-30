namespace Calmska.Tests.ApiTests.RepositoriesTests
{
    public class MoodRepositoryTests
    {
        private readonly MoodRepository _repository;
        private readonly Mock<IMapper> _mapper;
        private readonly CalmskaDbContext _context;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public MoodRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<CalmskaDbContext>()
               .UseInMemoryDatabase(databaseName: "TestDatabase")
               .Options;

            _context = new CalmskaDbContext(options);
            _mapper = new Mock<IMapper>();
            _repository = new MoodRepository(_context, _mapper.Object);
            _cancellationTokenSource = new CancellationTokenSource();
        }
        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedResult()
        {
            _context.Moods.RemoveRange(_context.Moods.ToList());
            _context.Moods.AddRange(new List<Mood>
            {
                new Mood { MoodId = Guid.NewGuid(), MoodName = "Happy", MoodTypeId = 1 },
                new Mood { MoodId = Guid.NewGuid(), MoodName = "Sad", MoodTypeId = 2 },
            });
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync(1, 2, _cancellationTokenSource.Token);

            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
        }
        [Fact]
        public async Task GetAllByArgumentAsync_ShouldReturnFilteredAndPaginatedResult()
        {
            _context.Moods.RemoveRange(_context.Moods.ToList());
            _context.Moods.AddRange(new List<Mood>
            {
                new Mood { MoodId = Guid.NewGuid(), MoodName = "Happy", MoodTypeId = 1 },
                new Mood { MoodId = Guid.NewGuid(), MoodName = "Sad", MoodTypeId = 2 },
            });
            await _context.SaveChangesAsync();

            var moodDTO = new MoodDTO { MoodName = "Happy" };

            var result = await _repository.GetAllByArgumentAsync(moodDTO, 1, 2, _cancellationTokenSource.Token);

            result.Items.Should().HaveCount(1);
            result.Items.First().MoodName.Should().Be("Happy");
            result.TotalCount.Should().Be(1);
        }
        [Fact]
        public async Task GetByArgumentAsync_ShouldReturnMatchingMood()
        {
            _context.Moods.RemoveRange(_context.Moods.ToList());
            var mood = new Mood { MoodId = Guid.NewGuid(), MoodName = "Excited", MoodTypeId = 1 };
            _context.Moods.Add(mood);
            await _context.SaveChangesAsync();

            var moodDTO = new MoodDTO { MoodId = mood.MoodId };

            var result = await _repository.GetByArgumentAsync(moodDTO, _cancellationTokenSource.Token);

            result.Should().NotBeNull();
            result!.MoodName.Should().Be("Excited");
        }
        [Fact]
        public async Task AddAsync_ShouldReturnError_WhenMoodDtoIsNull()
        {
            _context.Moods.RemoveRange(_context.Moods.ToList());
            MoodDTO moodDTO = null;

            var result = await _repository.AddAsync(moodDTO, _cancellationTokenSource.Token);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided Mood object is null.");
        }
        [Fact]
        public async Task AddAsync_ShouldReturnSuccess_WhenMoodIsAdded()
        {
            _context.Moods.RemoveRange(_context.Moods.ToList());
            var moodDTO = new MoodDTO { MoodName = "Happy", MoodTypeId = 1 };
            _mapper.Setup(m => m.Map<Mood>(It.IsAny<MoodDTO>()))
                       .Returns(new Mood { MoodName = moodDTO.MoodName, MoodTypeId = moodDTO.MoodTypeId });

            var result = await _repository.AddAsync(moodDTO, _cancellationTokenSource.Token);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();
            _context.Moods.Should().ContainSingle(m => m.MoodName == moodDTO.MoodName && m.MoodTypeId == moodDTO.MoodTypeId);
        }
        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenMoodDtoIsNull()
        {
            _context.Moods.RemoveRange(_context.Moods.ToList());
            MoodDTO moodDTO = null;

            var result = await _repository.UpdateAsync(moodDTO, _cancellationTokenSource.Token);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided Mood object is null.");
        }
        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenMoodNotFound()
        {
            _context.Moods.RemoveRange(_context.Moods.ToList());
            var moodDTO = new MoodDTO { MoodId = Guid.NewGuid(), MoodName = "Calm", MoodTypeId = 3 };

            var result = await _repository.UpdateAsync(moodDTO, _cancellationTokenSource.Token);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("Didn't find any mood with the provided moodId.");
        }
        [Fact]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenMoodIsUpdated()
        {
            _context.Moods.RemoveRange(_context.Moods.ToList());
            var existingMood = new Mood { MoodId = Guid.NewGuid(), MoodName = "Anxious", MoodTypeId = 2 };
            _context.Moods.Add(existingMood);
            await _context.SaveChangesAsync();

            var moodDTO = new MoodDTO { MoodId = existingMood.MoodId, MoodName = "Relaxed", MoodTypeId = 1 };
            var result = await _repository.UpdateAsync(moodDTO, _cancellationTokenSource.Token);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();

            var updatedMood = await _context.Moods.FirstOrDefaultAsync(m => m.MoodId == existingMood.MoodId);
            updatedMood.Should().NotBeNull();
            updatedMood!.MoodName.Should().Be("Relaxed");
            updatedMood.MoodTypeId.Should().Be(1);
        }
        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenMoodIdIsEmpty()
        {
            _context.Moods.RemoveRange(_context.Moods.ToList());
            var moodId = Guid.Empty;

            var result = await _repository.DeleteAsync(moodId, _cancellationTokenSource.Token);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided MoodId is null.");
        }
        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenMoodNotFound()
        {
            _context.Moods.RemoveRange(_context.Moods.ToList());
            var moodId = Guid.NewGuid();

            var result = await _repository.DeleteAsync(moodId, _cancellationTokenSource.Token);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("There is no mood with provided id.");
        }
        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenMoodIsDeleted()
        {
            _context.Moods.RemoveRange(_context.Moods.ToList());
            var existingMood = new Mood { MoodId = Guid.NewGuid(), MoodName = "Joyful", MoodTypeId = 1 };
            _context.Moods.Add(existingMood);
            await _context.SaveChangesAsync();

            var result = await _repository.DeleteAsync(existingMood.MoodId, _cancellationTokenSource.Token);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();
            _context.Moods.Should().NotContain(m => m.MoodId == existingMood.MoodId);
        }


    }
}
