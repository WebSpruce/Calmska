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
                new Tips { TipId = Guid.NewGuid(), Content = "Tip 1", TipsTypeId = 4 },
                new Tips { TipId = Guid.NewGuid(), Content = "Tip 2", TipsTypeId = 3 },
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
                new Tips { TipId = Guid.NewGuid(), Content = "Tip A", TipsTypeId = 3 },
                new Tips { TipId = Guid.NewGuid(), Content = "Tip B", TipsTypeId = 4 },
            });
            await _context.SaveChangesAsync();

            var tipsDto = new TipsDTO { TipsTypeId = 3 };

            var result = await _repository.GetAllByArgumentAsync(tipsDto, 1, 2);

            result.Items.Should().HaveCount(1);
            result.Items.First().TipsTypeId.Should().Be(3);
            result.TotalCount.Should().Be(1);
        }
        [Fact]
        public async Task GetByArgumentAsync_ShouldReturnMatchingTip()
        {
            _context.TipsDb.RemoveRange(_context.TipsDb.ToList());
            var tip = new Tips { TipId = Guid.NewGuid(), Content = "Find Me", TipsTypeId = 4 };
            _context.TipsDb.Add(tip);
            await _context.SaveChangesAsync();

            var tipsDto = new TipsDTO { TipId = tip.TipId };

            var result = await _repository.GetByArgumentAsync(tipsDto);

            result.Should().NotBeNull();
            result!.Content.Should().Be("Find Me");
            result.TipsTypeId.Should().Be(4);
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
            var tipsDto = new TipsDTO { Content = "Sample Tip", TipsTypeId = 3 };
            _mapper.Setup(m => m.Map<Tips>(It.IsAny<TipsDTO>()))
                       .Returns(new Tips { Content = tipsDto.Content, TipsTypeId = (int)tipsDto.TipsTypeId });

            var result = await _repository.AddAsync(tipsDto);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();
            _context.TipsDb.Should().ContainSingle(t => t.Content == tipsDto.Content && t.TipsTypeId == tipsDto.TipsTypeId);
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
            var existingTip = new Tips { TipId = Guid.NewGuid(), Content = "Old Tip", TipsTypeId = 3 };
            _context.TipsDb.Add(existingTip);
            await _context.SaveChangesAsync();

            var tipsDto = new TipsDTO { TipId = existingTip.TipId, Content = "Updated Tip", TipsTypeId = 4 };
            var result = await _repository.UpdateAsync(tipsDto);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();

            var updatedTip = await _context.TipsDb.FirstOrDefaultAsync(t => t.TipId == existingTip.TipId);
            updatedTip.Should().NotBeNull();
            updatedTip!.Content.Should().Be("Updated Tip");
            updatedTip.TipsTypeId.Should().Be(4);
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
            var existingTip = new Tips { TipId = Guid.NewGuid(), Content = "Tip to Delete", TipsTypeId = 1 };
            _context.TipsDb.Add(existingTip);
            await _context.SaveChangesAsync();

            var result = await _repository.DeleteAsync(existingTip.TipId);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();
            _context.TipsDb.Should().NotContain(t => t.TipId == existingTip.TipId);
        }
    }
}
