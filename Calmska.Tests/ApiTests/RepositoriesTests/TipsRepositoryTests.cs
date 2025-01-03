namespace Calmska.Tests.ApiTests.RepositoriesTests
{
    public class TipsRepositoryTests
    {
        private readonly TipsRepository _repository;
        private readonly Mock<IMapper> _mapper;
        private readonly CalmskaDbContext _context;

        public TipsRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<CalmskaDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new CalmskaDbContext(options);
            _mapper = new Mock<IMapper>();
            _repository = new TipsRepository(_context, _mapper.Object);
        }
        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedResult()
        {
            _context.TipsDb.RemoveRange(_context.TipsDb.ToList());
            _context.TipsDb.AddRange(new List<Tips>
            {
                new Tips { TipId = Guid.NewGuid(), Content = "Tip 1", Type = "General" },
                new Tips { TipId = Guid.NewGuid(), Content = "Tip 2", Type = "Specific" },
            });
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync(1, 2);

            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
        }
        [Fact]
        public async Task GetAllByArgumentAsync_ShouldReturnFilteredAndPaginatedResult()
        {
            _context.TipsDb.RemoveRange(_context.TipsDb.ToList());
            _context.TipsDb.AddRange(new List<Tips>
            {
                new Tips { TipId = Guid.NewGuid(), Content = "Tip A", Type = "General" },
                new Tips { TipId = Guid.NewGuid(), Content = "Tip B", Type = "Specific" },
            });
            await _context.SaveChangesAsync();

            var tipsDto = new TipsDTO { Type = "General" };

            var result = await _repository.GetAllByArgumentAsync(tipsDto, 1, 2);

            result.Items.Should().HaveCount(1);
            result.Items.First().Type.Should().Be("General");
            result.TotalCount.Should().Be(1);
        }
        [Fact]
        public async Task GetByArgumentAsync_ShouldReturnMatchingTip()
        {
            _context.TipsDb.RemoveRange(_context.TipsDb.ToList());
            var tip = new Tips { TipId = Guid.NewGuid(), Content = "Find Me", Type = "Target" };
            _context.TipsDb.Add(tip);
            await _context.SaveChangesAsync();

            var tipsDto = new TipsDTO { TipId = tip.TipId };

            var result = await _repository.GetByArgumentAsync(tipsDto);

            result.Should().NotBeNull();
            result!.Content.Should().Be("Find Me");
            result.Type.Should().Be("Target");
        }
        [Fact]
        public async Task AddAsync_ShouldReturnError_WhenTipsDtoIsNull()
        {
            _context.TipsDb.RemoveRange(_context.TipsDb.ToList());
            TipsDTO tipsDto = null;

            var result = await _repository.AddAsync(tipsDto);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided Tips object is null.");
        }
        [Fact]
        public async Task AddAsync_ShouldReturnSuccess_WhenTipIsAdded()
        {
            _context.TipsDb.RemoveRange(_context.TipsDb.ToList());
            var tipsDto = new TipsDTO { Content = "Sample Tip", Type = "General" };
            _mapper.Setup(m => m.Map<Tips>(It.IsAny<TipsDTO>()))
                       .Returns(new Tips { Content = tipsDto.Content, Type = tipsDto.Type });

            var result = await _repository.AddAsync(tipsDto);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();
            _context.TipsDb.Should().ContainSingle(t => t.Content == tipsDto.Content && t.Type == tipsDto.Type);
        }
        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenTipsDtoIsNull()
        {
            _context.TipsDb.RemoveRange(_context.TipsDb.ToList());
            TipsDTO tipsDto = null;

            var result = await _repository.UpdateAsync(tipsDto);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided Account object is null.");
        }
        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenTipNotFound()
        {
            _context.TipsDb.RemoveRange(_context.TipsDb.ToList());
            var tipsDto = new TipsDTO { TipId = Guid.NewGuid(), Content = "Updated Tip" };

            var result = await _repository.UpdateAsync(tipsDto);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("Didn't find any tip with the provided tipsId.");
        }
        [Fact]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenTipIsUpdated()
        {
            _context.TipsDb.RemoveRange(_context.TipsDb.ToList());
            var existingTip = new Tips { TipId = Guid.NewGuid(), Content = "Old Tip", Type = "General" };
            _context.TipsDb.Add(existingTip);
            await _context.SaveChangesAsync();

            var tipsDto = new TipsDTO { TipId = existingTip.TipId, Content = "Updated Tip", Type = "Specific" };
            var result = await _repository.UpdateAsync(tipsDto);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();

            var updatedTip = await _context.TipsDb.FirstOrDefaultAsync(t => t.TipId == existingTip.TipId);
            updatedTip.Should().NotBeNull();
            updatedTip!.Content.Should().Be("Updated Tip");
            updatedTip.Type.Should().Be("Specific");
        }
        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenTipsIdIsEmpty()
        {
            _context.TipsDb.RemoveRange(_context.TipsDb.ToList());
            var tipsId = Guid.Empty;

            var result = await _repository.DeleteAsync(tipsId);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided TipsId is null.");
        }
        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenTipNotFound()
        {
            _context.TipsDb.RemoveRange(_context.TipsDb.ToList());
            var tipsId = Guid.NewGuid();

            var result = await _repository.DeleteAsync(tipsId);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("There is no tip with provided id.");
        }
        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenTipIsDeleted()
        {
            _context.TipsDb.RemoveRange(_context.TipsDb.ToList());
            var existingTip = new Tips { TipId = Guid.NewGuid(), Content = "Tip to Delete", Type = "Sample" };
            _context.TipsDb.Add(existingTip);
            await _context.SaveChangesAsync();

            var result = await _repository.DeleteAsync(existingTip.TipId);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();
            _context.TipsDb.Should().NotContain(t => t.TipId == existingTip.TipId);
        }
    }
}
