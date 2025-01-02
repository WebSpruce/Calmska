namespace Calmska.Tests.ApiTests.RepositoriesTests
{
    public class SettingsRepositoryTests
    {
        private readonly SettingsRepository _repository;
        private readonly Mock<IMapper> _mapper;
        private readonly CalmskaDbContext _context;

        public SettingsRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<CalmskaDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new CalmskaDbContext(options);
            _mapper = new Mock<IMapper>();
            _repository = new SettingsRepository(_context, _mapper.Object);
        }
        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedResult()
        {
            _context.RemoveRange(_context.SettingsDb.ToList());
            _context.SettingsDb.AddRange(new List<Settings>
            {
                new Settings { SettingsId = Guid.NewGuid(), Color = "Blue", UserId = Guid.NewGuid(), PomodoroBreak = "5", PomodoroTimer = "45" },
                new Settings { SettingsId = Guid.NewGuid(), Color = "Red", UserId = Guid.NewGuid(), PomodoroBreak = "5", PomodoroTimer = "45" },
            });
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync(1, 2);

            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
        }
        [Fact]
        public async Task GetAllByArgumentAsync_ShouldReturnFilteredAndPaginatedResult()
        {
            _context.RemoveRange(_context.SettingsDb.ToList());
            _context.SettingsDb.AddRange(new List<Settings>
            {
                new Settings { SettingsId = Guid.NewGuid(), Color = "Blue", UserId = Guid.NewGuid(), PomodoroBreak = "5", PomodoroTimer = "45" },
                new Settings { SettingsId = Guid.NewGuid(), Color = "Red", UserId = Guid.NewGuid(), PomodoroBreak = "5", PomodoroTimer = "45" },
            });
            await _context.SaveChangesAsync();

            var settingsDTO = new SettingsDTO { Color = "Blue", PomodoroBreak = null, PomodoroTimer = null };

            var result = await _repository.GetAllByArgumentAsync(settingsDTO, 1, 2);

            result.Items.Should().HaveCount(1);
            result.Items.First().Color.Should().Be("Blue");
            result.TotalCount.Should().Be(1);
        }
        [Fact]
        public async Task GetByArgumentAsync_ShouldReturnMatchingSettings()
        {
            _context.RemoveRange(_context.SettingsDb.ToList());
            var settings = new Settings { SettingsId = Guid.NewGuid(), Color = "Yellow", UserId = Guid.NewGuid(), PomodoroBreak = "5", PomodoroTimer = "45" };
            _context.SettingsDb.Add(settings);
            await _context.SaveChangesAsync();

            var settingsDTO = new SettingsDTO { SettingsId = settings.SettingsId };

            var result = await _repository.GetByArgumentAsync(settingsDTO);

            result.Should().NotBeNull();
            result!.Color.Should().Be("Yellow");
        }

        [Fact]
        public async Task AddAsync_ShouldReturnError_WhenSettingsDtoIsNull()
        {
            _context.RemoveRange(_context.SettingsDb.ToList());
            SettingsDTO settingsDTO = null;

            var result = await _repository.AddAsync(settingsDTO);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided Settings object is null.");
        }
        [Fact]
        public async Task AddAsync_ShouldReturnError_WhenSettingsAlreadyExistsForUser()
        {
            _context.RemoveRange(_context.SettingsDb.ToList());
            var existingSettings = new Settings { SettingsId = Guid.NewGuid(), UserId = Guid.NewGuid(), Color = "Blue", PomodoroBreak = "5", PomodoroTimer = "45" };
            _context.SettingsDb.Add(existingSettings);
            await _context.SaveChangesAsync();

            var settingsDTO = new SettingsDTO { UserId = existingSettings.UserId, Color = "Red" };

            var result = await _repository.AddAsync(settingsDTO);

            result.Result.Should().BeFalse();
            result.Error.Should().Contain("The settings object exists for the user with id:");
        }
        [Fact]
        public async Task AddAsync_ShouldReturnSuccess_WhenSettingsIsAdded()
        {
            _context.RemoveRange(_context.SettingsDb.ToList());
            var settingsDTO = new SettingsDTO { UserId = Guid.NewGuid(), Color = "Green" };
            _mapper.Setup(m => m.Map<Settings>(It.IsAny<SettingsDTO>()))
                       .Returns(new Settings { UserId = settingsDTO.UserId ?? Guid.Empty, Color = settingsDTO.Color, PomodoroBreak = "5", PomodoroTimer = "45" });

            var result = await _repository.AddAsync(settingsDTO);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();
            _context.SettingsDb.Should().ContainSingle(s => s.Color == settingsDTO.Color && s.UserId == settingsDTO.UserId);
        }
        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenSettingsDtoIsNull()
        {
            _context.RemoveRange(_context.SettingsDb.ToList());
            SettingsDTO settingsDTO = null;

            var result = await _repository.UpdateAsync(settingsDTO);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided Settings object is null.");
        }
        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenSettingsNotFound()
        {
            _context.RemoveRange(_context.SettingsDb.ToList());
            var settingsDTO = new SettingsDTO { SettingsId = Guid.NewGuid(), Color = "Yellow" };

            var result = await _repository.UpdateAsync(settingsDTO);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("Didn't find any settings with the provided settingsId.");
        }
        [Fact]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenSettingsIsUpdated()
        {
            _context.RemoveRange(_context.SettingsDb.ToList());
            var existingSettings = new Settings { UserId = Guid.NewGuid(), SettingsId = Guid.NewGuid(), Color = "Blue", PomodoroBreak = "5", PomodoroTimer = "45" };
            _context.SettingsDb.Add(existingSettings);
            await _context.SaveChangesAsync();

            var settingsDTO = new SettingsDTO { SettingsId = existingSettings.SettingsId, Color = "Green" };

            var result = await _repository.UpdateAsync(settingsDTO);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();

            var updatedSettings = await _context.SettingsDb.FirstOrDefaultAsync(s => s.SettingsId == existingSettings.SettingsId);
            updatedSettings.Should().NotBeNull();
            updatedSettings!.Color.Should().Be("Green");
        }
        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenSettingsIdIsEmpty()
        {
            _context.RemoveRange(_context.SettingsDb.ToList());
            var settingsId = Guid.Empty;

            var result = await _repository.DeleteAsync(settingsId);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided SettingsId is null.");
        }
        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenSettingsNotFound()
        {
            _context.RemoveRange(_context.SettingsDb.ToList());
            var settingsId = Guid.NewGuid();

            var result = await _repository.DeleteAsync(settingsId);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("There is no settings with provided id.");
        }
        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenSettingsIsDeleted()
        {
            _context.RemoveRange(_context.SettingsDb.ToList());
            var existingSettings = new Settings { UserId = Guid.NewGuid(), SettingsId = Guid.NewGuid(), Color = "Blue", PomodoroBreak = "5", PomodoroTimer = "45" };
            _context.SettingsDb.Add(existingSettings);
            await _context.SaveChangesAsync();

            var result = await _repository.DeleteAsync(existingSettings.SettingsId);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();
            _context.SettingsDb.Should().NotContain(s => s.SettingsId == existingSettings.SettingsId);
        }
    }
}
