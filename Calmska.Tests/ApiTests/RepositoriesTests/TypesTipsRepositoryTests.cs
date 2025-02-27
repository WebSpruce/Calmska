namespace Calmska.Tests.ApiTests.RepositoriesTests
{
    public class TypesTipsRepositoryTests
    {
        private readonly Types_TipsRepository _repository;
        private readonly Mock<IMapper> _mapper;
        private readonly CalmskaDbContext _context;

        public TypesTipsRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<CalmskaDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new CalmskaDbContext(options);
            _mapper = new Mock<IMapper>();
            _repository = new Types_TipsRepository(_context, _mapper.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedResult()
        {
            _context.Types_TipsDb.RemoveRange(_context.Types_TipsDb.ToList());
            _context.Types_TipsDb.AddRange(new List<Types_Tips>
            {
                new Types_Tips { TypeId = 1, Type = "Type 1" },
                new Types_Tips { TypeId = 2, Type = "Type 2" }
            });
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync(1, 2);

            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
        }

        [Fact]
        public async Task GetAllByArgumentAsync_ShouldReturnFilteredAndPaginatedResult()
        {
            _context.Types_TipsDb.RemoveRange(_context.Types_TipsDb.ToList());
            _context.Types_TipsDb.AddRange(new List<Types_Tips>
            {
                new Types_Tips { TypeId = 1, Type = "Category A" },
                new Types_Tips { TypeId = 2, Type = "Category B" }
            });
            await _context.SaveChangesAsync();

            var filterDto = new Types_TipsDTO { Type = "Category A" };

            var result = await _repository.GetAllByArgumentAsync(filterDto, 1, 2);

            result.Items.Should().HaveCount(1);
            result.Items.First().Type.Should().Be("Category A");
            result.TotalCount.Should().Be(1);
        }

        [Fact]
        public async Task GetByArgumentAsync_ShouldReturnMatchingTypeTip()
        {
            _context.Types_TipsDb.RemoveRange(_context.Types_TipsDb.ToList());
            var typeTip = new Types_Tips { TypeId = 3, Type = "Find Me" };
            _context.Types_TipsDb.Add(typeTip);
            await _context.SaveChangesAsync();

            var filterDto = new Types_TipsDTO { TypeId = 3 };

            var result = await _repository.GetByArgumentAsync(filterDto);

            result.Should().NotBeNull();
            result!.Type.Should().Be("Find Me");
        }

        [Fact]
        public async Task AddAsync_ShouldReturnError_WhenTypesDtoIsNull()
        {
            _context.Types_TipsDb.RemoveRange(_context.Types_TipsDb.ToList());
            Types_TipsDTO dto = null;

            var result = await _repository.AddAsync(dto);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided Type_Tip object is null.");
        }

        [Fact]
        public async Task AddAsync_ShouldReturnSuccess_WhenTypeTipIsAdded()
        {
            _context.Types_TipsDb.RemoveRange(_context.Types_TipsDb.ToList());
            var dto = new Types_TipsDTO { Type = "New Type" };
            _mapper.Setup(m => m.Map<Types_Tips>(It.IsAny<Types_TipsDTO>()))
                .Returns(new Types_Tips { Type = dto.Type });

            var result = await _repository.AddAsync(dto);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();
            _context.Types_TipsDb.Should().ContainSingle(t => t.Type == dto.Type);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenTypesDtoIsNull()
        {
            Types_TipsDTO dto = null;

            var result = await _repository.UpdateAsync(dto);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided Type_Tip object is null.");
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenTypeTipNotFound()
        {
            var dto = new Types_TipsDTO { TypeId = 99, Type = "Updated Type" };

            var result = await _repository.UpdateAsync(dto);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("Didn't find any type_tip with the provided type_tipsId.");
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenTypeTipIsUpdated()
        {
            var existingTypeTip = new Types_Tips { TypeId = 4, Type = "Old Type" };
            _context.Types_TipsDb.Add(existingTypeTip);
            await _context.SaveChangesAsync();

            var dto = new Types_TipsDTO { TypeId = 4, Type = "Updated Type" };
            var result = await _repository.UpdateAsync(dto);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();

            var updatedTip = await _context.Types_TipsDb.FirstOrDefaultAsync(t => t.TypeId == existingTypeTip.TypeId);
            updatedTip.Should().NotBeNull();
            updatedTip!.Type.Should().Be("Updated Type");
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenTypeTipIdIsInvalid()
        {
            var result = await _repository.DeleteAsync(0);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("The provided type_tipId is <= 0.");
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenTypeTipNotFound()
        {
            var result = await _repository.DeleteAsync(99);

            result.Result.Should().BeFalse();
            result.Error.Should().Be("There is no type_tip with provided id.");
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenTypeTipIsDeleted()
        {
            var existingTypeTip = new Types_Tips { TypeId = 5, Type = "Delete Me" };
            _context.Types_TipsDb.Add(existingTypeTip);
            await _context.SaveChangesAsync();

            var result = await _repository.DeleteAsync(existingTypeTip.TypeId);

            result.Result.Should().BeTrue();
            result.Error.Should().BeEmpty();
            _context.Types_TipsDb.Should().NotContain(t => t.TypeId == existingTypeTip.TypeId);
        }
    }
}
