using Calmska.Models.Models;

namespace Calmska.Services.Interfaces
{
    public interface IService<T>
    {
        /// <summary>
        /// Get all objects with optional pagination.
        /// </summary>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The page size for pagination.</param>
        /// <returns>A paginated result containing a list of objects.</returns>
        Task<OperationResultT<IEnumerable<T?>>> GetAllAsync(int? pageNumber, int? pageSize);

        /// <summary>
        /// Search for objects using specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria for searching objects.</param>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The page size for pagination.</param>
        /// <returns>A paginated result containing matched objects.</returns>
        Task<OperationResultT<PaginatedResult<IEnumerable<T?>>>> SearchAllByArgumentAsync(T criteria, int? pageNumber, int? pageSize);

        /// <summary>
        /// Get a specific object by an argument.
        /// </summary>
        /// <param name="criteria">The criteria for searching object.</param>
        /// <returns>The object if found, or null if not.</returns>
        Task<OperationResultT<T?>> GetByArgumentAsync(T criteria);

        /// <summary>
        /// Add a new object.
        /// </summary>
        /// <param name="newObject">The details of the object to add.</param>
        /// <returns>The result of the add operation.</returns>
        Task<OperationResultT<bool>> AddAsync(T newObject);

        /// <summary>
        /// Update an existing object.
        /// </summary>
        /// <param name="updatedObject">The updated details of the object.</param>
        /// <returns>The result of the update operation.</returns>
        Task<OperationResultT<bool>> UpdateAsync(T updatedObject);

        /// <summary>
        /// Delete an object by its unique identifier.
        /// </summary>
        /// <param name="objectId">The unique identifier of the object to delete.</param>
        /// <returns>The result of the delete operation.</returns>
        Task<OperationResultT<bool>> DeleteAsync(Guid objectId);
    }
}
