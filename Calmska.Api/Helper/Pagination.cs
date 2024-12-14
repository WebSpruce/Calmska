using Calmska.Models.Models;

namespace Calmska.Api.Helper
{
    public static class Pagination
    {
        public static PaginatedResult<T> Paginate<T>(IQueryable<T> query, int? pageNumber, int? pageSize)
        {
            // Validate query
            if (query == null || !query.Any())
            {
                return new PaginatedResult<T>
                {
                    Items = new List<T>(),
                    TotalCount = 0,
                    PageNumber = 0,
                    PageSize = 0,
                    error = "No data found for the given criteria."
                };
            }

            // Validate pageSize
            if (pageSize.HasValue && pageSize <= 0)
            {
                return new PaginatedResult<T>
                {
                    Items = new List<T>(),
                    TotalCount = 0,
                    PageNumber = 0,
                    PageSize = 0,
                    error = "Page size must be greater than 0."
                };
            }

            try
            {
                int currentPage = pageNumber ?? 1;
                int currentPageSize = pageSize ?? query.Count();

                if (currentPage <= 0)
                {
                    return new PaginatedResult<T>
                    {
                        Items = new List<T>(),
                        TotalCount = 0,
                        PageNumber = 0,
                        PageSize = 0,
                        error = "Page number must be greater than 0."
                    };
                }

                var totalItems = query.Count();
                var items = query
                    .Skip((currentPage - 1) * currentPageSize)
                    .Take(currentPageSize)
                    .ToList();

                return new PaginatedResult<T>
                {
                    Items = items,
                    TotalCount = totalItems,
                    PageNumber = currentPage,
                    PageSize = currentPageSize
                };
            }
            catch(Exception ex)
            {
                return new PaginatedResult<T>
                {
                    Items = new List<T>(),
                    TotalCount = 0,
                    PageNumber = 0,
                    PageSize = 0,
                    error = ex.Message
                };
            }
        }
    }
}
