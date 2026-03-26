using AutoMapper;
using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using Calmska.Infrastructure.Persistence.Models;
using Calmska.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace Calmska.Infrastructure.Persistence.Repositories
{
    public class AccountRepository : IRepository<Account, AccountFilter>
    {
        private readonly CalmskaDbContext _context;
        private readonly IMapper _mapper;
        
        public AccountRepository(CalmskaDbContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }
       
        public async Task<PaginatedResult<Account>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await Task.Run(_context.Accounts.AsQueryable, token);
            
            var documentResult = Pagination.Paginate(query, pageNumber, pageSize);
            var domainItems = _mapper.Map<IEnumerable<Account>>(documentResult.Items);
            
            return new PaginatedResult<Account>(domainItems, documentResult.TotalCount, documentResult.PageNumber, documentResult.PageSize);
        }

        public async Task<PaginatedResult<Account>> GetAllByArgumentAsync(AccountFilter account, int? pageNumber, int? pageSize,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await Task.Run(_context.Accounts
            .Where(item =>
                (!account.UserId.HasValue || item.UserId == account.UserId) &&
                (string.IsNullOrEmpty(account.UserName) || (item.UserName != null && item.UserName.ToLower().Contains(account.UserName.ToLower())) ) &&
                (string.IsNullOrEmpty(account.Email) || (item.Email != null && item.Email.ToLower().Contains(account.Email.ToLower())) ) &&
                (string.IsNullOrEmpty(account.PasswordHashed) || item.PasswordHashed == account.PasswordHashed)
            )
            .AsQueryable, token);
            
            var documentResult = Pagination.Paginate(query, pageNumber, pageSize);
            var domainItems = _mapper.Map<IEnumerable<Account>>(documentResult.Items);
            
            return new PaginatedResult<Account>(domainItems, documentResult.TotalCount, documentResult.PageNumber, documentResult.PageSize);
        }

        public async Task<Account?> GetByArgumentAsync(AccountFilter account, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await _context.Accounts
                .Where(item =>
                    (!account.UserId.HasValue || item.UserId == account.UserId) &&
                    (string.IsNullOrEmpty(account.UserName) || (item.UserName != null && item.UserName.ToLower().Contains(account.UserName.ToLower())) ) &&
                    (string.IsNullOrEmpty(account.Email) || (item.Email != null && item.Email.ToLower().Contains(account.Email.ToLower())) ) &&
                    (string.IsNullOrEmpty(account.PasswordHashed) || item.PasswordHashed == account.PasswordHashed)
                )
                .FirstOrDefaultAsync(token);
            
            return _mapper.Map<Account?>(query);
        }
        public async Task<OperationResult> AddAsync(Account account, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var document = _mapper.Map<AccountDocument>(account);

                await _context.Accounts.AddAsync(document, token);
                var result = await _context.SaveChangesAsync(token);

                return new OperationResult
                {
                    Result = result > 0,
                    Error = string.Empty
                };
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Result = false,
                    Error = $"{ex.Message} - {ex.InnerException}"
                };
            }
        }

        public async Task<OperationResult> UpdateAsync(AccountFilter account, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                
                var existingAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == account.UserId, token);
                if(existingAccount == null)
                    return new OperationResult { Result = false, Error = "Didn't find any account with the provided userId." };

                if(!string.IsNullOrEmpty(account.UserName))
                    existingAccount.UserName = account.UserName;
                if(!string.IsNullOrEmpty(account.Email))
                    existingAccount.Email = account.Email;
                if (!string.IsNullOrEmpty(account.PasswordHashed))
                    existingAccount.PasswordHashed = BCrypt.Net.BCrypt.HashPassword(account.PasswordHashed);

                await _context.SaveChangesAsync(token);
                return new OperationResult { Result = true, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> DeleteAsync(Guid accountId, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var accountObject = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == accountId, token);
                if (accountObject == null)
                    return new OperationResult { Result = false, Error = "There is no settings with provided id." };

                _context.Accounts.Remove(accountObject);

                await _context.SaveChangesAsync(token);
                return new OperationResult { Result = true, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }
    }
}
