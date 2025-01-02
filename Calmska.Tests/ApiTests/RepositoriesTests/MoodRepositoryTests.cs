namespace Calmska.Tests.ApiTests.RepositoriesTests
{
    public class MoodRepositoryTests
    {
        private readonly MoodRepository _repository;
        private readonly Mock<IMapper> _mapper;
        private readonly CalmskaDbContext _context;

        public MoodRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<CalmskaDbContext>()
               .UseInMemoryDatabase(databaseName: "TestDatabase")
               .Options;

            _context = new CalmskaDbContext(options);
            _mapper = new Mock<IMapper>();
            _repository = new MoodRepository(_context, _mapper.Object);
        }
        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedResult()
        {
            _context.Moods.RemoveRange(_context.Moods.ToList());
            _context.Moods.AddRange(new List<Mood>
            {
                new Mood { MoodId = Guid.NewGuid(), MoodName = "Happy", Type = "Positive" },
                new Mood { MoodId = Guid.NewGuid(), MoodName = "Sad", Type = "Negative" },
            });
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync(1, 2);

            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
        }
        [Fact]
        public async Task GetAllByArgumentAsync_ShouldReturnFilteredAndPaginatedResult()
        {
            _context.Moods.RemoveRange(_context.Moods.ToList());
            _context.Moods.AddRange(new List<Mood>
            {
                new Mood { MoodId = Guid.NewGuid(), MoodName = "Happy", Type = "Positive" },
                new Mood { MoodId = Guid.NewGuid(), MoodName = "Sad", Type = "Negative" },
            });
            await _context.SaveChangesAsync();

            var moodDTO = new MoodDTO { MoodName = "Happy" };

            var result = await _repository.GetAllByArgumentAsync(moodDTO, 1, 2);

            result.Items.Should().HaveCount(1);
            result.Items.First().MoodName.Should().Be("Happy");
            result.TotalCount.Should().Be(1);
        }
        [Fact]
        public async Task GetByArgumentAsync_ShouldReturnMatchingMood()
        {
            _context.Moods.RemoveRange(_context.Moods.ToList());
            var mood = new Mood { MoodId = Guid.NewGuid(), MoodName = "Excited", Type = "Positive" };
            _context.Moods.Add(mood);
            await _context.SaveChangesAsync();

            var moodDTO = new MoodDTO { MoodId = mood.MoodId };

            var result = await _repository.GetByArgumentAsync(moodDTO);

            result.Should().NotBeNull();
            result!.MoodName.Should().Be("Excited");
        }
        [Fact]
        public async Task AddAsync_ShouldReturnError_WhenMoodDtoIsNull()
        {
            _context.Moods.RemoveRange(_context.Moods.ToList());
            MoodDTO moodDTO = null;

            var result = await _repository.AddAsync(moodDTO);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided Mood object is null.");
        }
        [Fact]
        public async Task AddAsync_ShouldReturnSuccess_WhenMoodIsAdded()
        {
            _context.Moods.RemoveRange(_context.Moods.ToList());
            var moodDTO = new MoodDTO { MoodName = "Happy", Type = "Positive" };
            _mapper.Setup(m => m.Map<Mood>(It.IsAny<MoodDTO>()))
                       .Returns(new Mood { MoodName = moodDTO.MoodName, Type = moodDTO.Type });

            var result = await _repository.AddAsync(moodDTO);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();
            _context.Moods.Should().ContainSingle(m => m.MoodName == moodDTO.MoodName && m.Type == moodDTO.Type);
        }
        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenMoodDtoIsNull()
        {
            _context.Moods.RemoveRange(_context.Moods.ToList());
            MoodDTO moodDTO = null;

            var result = await _repository.UpdateAsync(moodDTO);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided Mood object is null.");
        }
        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenMoodNotFound()
        {
            _context.Moods.RemoveRange(_context.Moods.ToList());
            var moodDTO = new MoodDTO { MoodId = Guid.NewGuid(), MoodName = "Calm", Type = "Neutral" };

            var result = await _repository.UpdateAsync(moodDTO);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("Didn't find any mood with the provided moodId.");
        }
        [Fact]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenMoodIsUpdated()
        {
            _context.Moods.RemoveRange(_context.Moods.ToList());
            var existingMood = new Mood { MoodId = Guid.NewGuid(), MoodName = "Anxious", Type = "Negative" };
            _context.Moods.Add(existingMood);
            await _context.SaveChangesAsync();

            var moodDTO = new MoodDTO { MoodId = existingMood.MoodId, MoodName = "Relaxed", Type = "Positive" };
            var result = await _repository.UpdateAsync(moodDTO);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();

            var updatedMood = await _context.Moods.FirstOrDefaultAsync(m => m.MoodId == existingMood.MoodId);
            updatedMood.Should().NotBeNull();
            updatedMood!.MoodName.Should().Be("Relaxed");
            updatedMood.Type.Should().Be("Positive");
        }
        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenMoodIdIsEmpty()
        {
            _context.Moods.RemoveRange(_context.Moods.ToList());
            var moodId = Guid.Empty;

            var result = await _repository.DeleteAsync(moodId);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided MoodId is null.");
        }
        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenMoodNotFound()
        {
            _context.Moods.RemoveRange(_context.Moods.ToList());
            var moodId = Guid.NewGuid();

            var result = await _repository.DeleteAsync(moodId);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("There is no mood with provided id.");
        }
        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenMoodIsDeleted()
        {
            _context.Moods.RemoveRange(_context.Moods.ToList());
            var existingMood = new Mood { MoodId = Guid.NewGuid(), MoodName = "Joyful", Type = "Positive" };
            _context.Moods.Add(existingMood);
            await _context.SaveChangesAsync();

            var result = await _repository.DeleteAsync(existingMood.MoodId);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();
            _context.Moods.Should().NotContain(m => m.MoodId == existingMood.MoodId);
        }


    }
}
