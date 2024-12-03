using Calmska.Models.Models;

namespace Calmska.Api.Helper
{
    public static class Pagination
    {
        public static PaginatedResult<T> Paginate<T>(IQueryable<T> query, int? pageNumber, int? pageSize)
        {
            int currentPage = pageNumber ?? 1;
            int currentPageSize = pageSize ?? query.Count();

            var items = query
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();

            return new PaginatedResult<T>
            {
                Items = items,
                TotalCount = query.Count(),
                PageNumber = currentPage,
                PageSize = currentPageSize
            };
        }
    }
}
