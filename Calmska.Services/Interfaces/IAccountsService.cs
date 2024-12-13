using Calmska.Models.DTO;
using Calmska.Models.Models;

namespace Calmska.Services.Interfaces
{
    public interface IAccountsService
    {
        /// <summary>
        /// Get all accounts with optional pagination.
        /// </summary>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The page size for pagination.</param>
        /// <returns>A paginated result containing a list of accounts.</returns>
        Task<OperationResultT<IEnumerable<AccountDTO?>>> GetAllAccountsAsync(int? pageNumber, int? pageSize);

        /// <summary>
        /// Search for accounts using specified criteria.
        /// </summary>
        /// <param name="accountCriteria">The criteria for searching accounts.</param>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The page size for pagination.</param>
        /// <returns>A paginated result containing matched accounts.</returns>
        Task<OperationResultT<PaginatedResult<IEnumerable<AccountDTO?>>>> SearchAccountsByArgumentAsync(AccountDTO accountCriteria, int? pageNumber, int? pageSize);

        /// <summary>
        /// Get a specific account by an argument.
        /// </summary>
        /// <param name="accountCriteria">The criteria for searching accounts.</param>
        /// <returns>The account if found, or null if not.</returns>
        Task<OperationResultT<AccountDTO?>> GetAccountByArgumentAsync(AccountDTO accountCriteria);

        /// <summary>
        /// Add a new account.
        /// </summary>
        /// <param name="newAccount">The details of the account to add.</param>
        /// <returns>The result of the add operation.</returns>
        Task<OperationResultT<bool>> AddAccountAsync(AccountDTO newAccount);

        /// <summary>
        /// Update an existing account.
        /// </summary>
        /// <param name="updatedAccount">The updated details of the account.</param>
        /// <returns>The result of the update operation.</returns>
        Task<OperationResultT<bool>> UpdateAccountAsync(AccountDTO updatedAccount);

        /// <summary>
        /// Delete an account by its unique identifier.
        /// </summary>
        /// <param name="accountId">The unique identifier of the account to delete.</param>
        /// <returns>The result of the delete operation.</returns>
        Task<OperationResultT<bool>> DeleteAccountAsync(Guid accountId);
    }
}
