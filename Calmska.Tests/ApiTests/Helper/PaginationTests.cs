namespace Calmska.Tests.ApiTests.Helper
{
    public class PaginationTests
    {
        [Fact]
        public void Paginate_ShouldReturnError_WhenQueryIsNull()
        {
            var result = Pagination.Paginate<int>(null, 1, 10);

            result.Should().NotBeNull();
            result.Items.Should().BeEmpty();
            result.TotalCount.Should().Be(0);
            result.PageNumber.Should().Be(0);
            result.PageSize.Should().Be(0);
            result.error.Should().Be("No data found for the given criteria.");
        }

        [Fact]
        public void Paginate_ShouldReturnError_WhenQueryIsEmpty()
        {
            IQueryable<int> query = Enumerable.Empty<int>().AsQueryable();

            var result = Pagination.Paginate(query, 1, 10);

            result.Items.Should().BeEmpty();
            result.TotalCount.Should().Be(0);
            result.error.Should().Be("No data found for the given criteria.");
        }

        [Fact]
        public void Paginate_ShouldReturnError_WhenPageNumberIsInvalid()
        {
            IQueryable<int> query = Enumerable.Range(1, 10).AsQueryable();

            var result = Pagination.Paginate(query, 0, 5);

            result.Items.Should().BeEmpty();
            result.PageNumber.Should().Be(0);
            result.error.Should().Be("Page number must be greater than 0.");
        }

        [Fact]
        public void Paginate_ShouldReturnError_WhenPageSizeIsInvalid()
        {
            IQueryable<int> query = Enumerable.Range(1, 10).AsQueryable();

            var result = Pagination.Paginate(query, 1, 0);

            result.Items.Should().BeEmpty();
            result.PageSize.Should().Be(0);
            result.error.Should().Be("Page size must be greater than 0.");
        }

        [Fact]
        public void Paginate_ShouldProvidePaginatedResults()
        {
            IQueryable<int> query = Enumerable.Range(1, 100).AsQueryable();
            int pageNumber = 2;
            int pageSize = 10;

            var result = Pagination.Paginate(query, pageNumber, pageSize);

            result.Items.Should().HaveCount(pageSize);
            result.Items.Should().ContainInOrder(Enumerable.Range(11, 10));
            result.TotalCount.Should().Be(query.Count());
            result.PageNumber.Should().Be(pageNumber);
            result.PageSize.Should().Be(pageSize);
            result.error.Should().BeNullOrEmpty();
        }
    }
}