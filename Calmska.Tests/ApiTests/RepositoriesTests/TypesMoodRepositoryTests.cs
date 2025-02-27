namespace Calmska.Tests.ApiTests.RepositoriesTests
{
    public class TypesMoodRepositoryTests
    {
        private readonly Types_MoodRepository _repository;
        private readonly Mock<IMapper> _mapper;
        private readonly CalmskaDbContext _context;

        public TypesMoodRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<CalmskaDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new CalmskaDbContext(options);
            _mapper = new Mock<IMapper>();
            _repository = new Types_MoodRepository(_context, _mapper.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedResult()
        {
            _context.Types_MoodDb.RemoveRange(_context.Types_MoodDb.ToList());
            _context.Types_MoodDb.AddRange(new List<Types_Mood>
            {
                new Types_Mood { TypeId = 1, Type = "Happy" },
                new Types_Mood { TypeId = 2, Type = "Sad" },
            });
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync(1, 2);

            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
        }

        [Fact]
        public async Task GetAllByArgumentAsync_ShouldReturnFilteredAndPaginatedResult()
        {
            _context.Types_MoodDb.RemoveRange(_context.Types_MoodDb.ToList());
            _context.Types_MoodDb.AddRange(new List<Types_Mood>
            {
                new Types_Mood { TypeId = 1, Type = "Excited" },
                new Types_Mood { TypeId = 2, Type = "Calm" },
            });
            await _context.SaveChangesAsync();

            var moodsDto = new Types_MoodDTO { Type = "Calm" };
            var result = await _repository.GetAllByArgumentAsync(moodsDto, 1, 2);

            result.Items.Should().HaveCount(1);
            result.Items.First().Type.Should().Be("Calm");
            result.TotalCount.Should().Be(1);
        }

        [Fact]
        public async Task GetByArgumentAsync_ShouldReturnMatchingMood()
        {
            _context.Types_MoodDb.RemoveRange(_context.Types_MoodDb.ToList());
            var mood = new Types_Mood { TypeId = 3, Type = "Relaxed" };
            _context.Types_MoodDb.Add(mood);
            await _context.SaveChangesAsync();

            var moodsDto = new Types_MoodDTO { TypeId = 3 };
            var result = await _repository.GetByArgumentAsync(moodsDto);

            result.Should().NotBeNull();
            result!.Type.Should().Be("Relaxed");
        }

        [Fact]
        public async Task AddAsync_ShouldReturnError_WhenMoodDtoIsNull()
        {
            Types_MoodDTO moodsDto = null;
            var result = await _repository.AddAsync(moodsDto);
            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided Type_mood object is null.");
        }

        [Fact]
        public async Task AddAsync_ShouldReturnSuccess_WhenMoodIsAdded()
        {
            var moodsDto = new Types_MoodDTO { Type = "Energetic" };
            _mapper.Setup(m => m.Map<Types_Mood>(It.IsAny<Types_MoodDTO>()))
                   .Returns(new Types_Mood { Type = moodsDto.Type });

            var result = await _repository.AddAsync(moodsDto);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();
            _context.Types_MoodDb.Should().ContainSingle(m => m.Type == moodsDto.Type);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenMoodDtoIsNull()
        {
            Types_MoodDTO moodsDto = null;
            var result = await _repository.UpdateAsync(moodsDto);
            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided Type_mood object is null.");
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenMoodIsUpdated()
        {
            var existingMood = new Types_Mood { TypeId = 4, Type = "Bored" };
            _context.Types_MoodDb.Add(existingMood);
            await _context.SaveChangesAsync();

            var moodsDto = new Types_MoodDTO { TypeId = 4, Type = "Excited" };
            var result = await _repository.UpdateAsync(moodsDto);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();

            var updatedMood = await _context.Types_MoodDb.FirstOrDefaultAsync(m => m.TypeId == existingMood.TypeId);
            updatedMood.Should().NotBeNull();
            updatedMood!.Type.Should().Be("Excited");
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenMoodIdIsInvalid()
        {
            var result = await _repository.DeleteAsync(0);
            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided type_moodId is <= 0.");
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenMoodIsDeleted()
        {
            var existingMood = new Types_Mood { TypeId = 5, Type = "Frustrated" };
            _context.Types_MoodDb.Add(existingMood);
            await _context.SaveChangesAsync();

            var result = await _repository.DeleteAsync(existingMood.TypeId);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();
            _context.Types_MoodDb.Should().NotContain(m => m.TypeId == existingMood.TypeId);
        }
    }
}
