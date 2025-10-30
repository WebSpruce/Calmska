using AutoMapper;
using Calmska.Models.DTO;
using Calmska.Api.Helper;
using Calmska.Api.Interfaces;
using Calmska.Models.Models;
using Microsoft.EntityFrameworkCore;
using Firebase.Auth;

namespace Calmska.Api.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly CalmskaDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFirebaseAuthClient _authClient;
        public AccountRepository(CalmskaDbContext context, IMapper mapper, IFirebaseAuthClient authClient) 
        {
            _context = context;
            _mapper = mapper;
            _authClient = authClient;
        }
       
        public async Task<PaginatedResult<Account>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await Task.Run(_context.Accounts.AsQueryable, token);
            return Pagination.Paginate(query, pageNumber, pageSize);
        }

        public async Task<PaginatedResult<Account>> GetAllByArgumentAsync(AccountDTO account, int? pageNumber, int? pageSize,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await Task.Run(_context.Accounts
            .Where(item =>
                (!account.UserId.HasValue || item.UserId == account.UserId) &&
                (string.IsNullOrEmpty(account.UserName) || item.UserName.ToLower().Contains(account.UserName.ToLower())) &&
                (string.IsNullOrEmpty(account.Email) || item.Email.ToLower().Contains(account.Email.ToLower())) &&
                (string.IsNullOrEmpty(account.PasswordHashed) || item.PasswordHashed == account.PasswordHashed)
            )
            .AsQueryable, token);

            return Pagination.Paginate(query, pageNumber, pageSize);
        }

        public async Task<Account?> GetByArgumentAsync(AccountDTO account, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return await _context.Accounts
                .Where(item =>
                    (!account.UserId.HasValue || item.UserId == account.UserId) &&
                    (string.IsNullOrEmpty(account.UserName) || item.UserName.ToLower().Contains(account.UserName.ToLower())) &&
                    (string.IsNullOrEmpty(account.Email) || item.Email.ToLower().Contains(account.Email.ToLower())) &&
                    (string.IsNullOrEmpty(account.PasswordHashed) || item.PasswordHashed == account.PasswordHashed)
                )
                .FirstOrDefaultAsync(token);
        }
        public async Task<bool> LoginAsync(AccountDTO account, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                var userExists = await _context.Accounts.FirstOrDefaultAsync(u => u.Email == account.Email, token);
                if (userExists == null)
                {
                    return false;
                }
                if (HashPassword.VerifyPassword(account.PasswordHashed, userExists.PasswordHashed))
                {
                    await _authClient.SignInWithEmailAndPasswordAsync(account.Email, userExists.PasswordHashed);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<OperationResult> AddAsync(AccountDTO accountDto, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                
                if (accountDto == null)
                    return new OperationResult { Result = false, Error = "The provided Account object is null." };

                var userByEmail = await GetByArgumentAsync(new AccountDTO { Email = accountDto.Email, PasswordHashed = string.Empty, UserName = string.Empty, UserId = null}, token);
                if (userByEmail != null)
                    return new OperationResult { Result = false, Error = "The user with provided email exists." };


                accountDto.PasswordHashed = HashPassword.SetHash(accountDto.PasswordHashed);
                //if firebase fails, it throws an exception
                var account = _mapper.Map<Account>(accountDto);
                await _context.Accounts.AddAsync(account, token);

                var result = await _context.SaveChangesAsync(token);
                if(result > 0)
                    await _authClient.CreateUserWithEmailAndPasswordAsync(accountDto.Email, accountDto.PasswordHashed);

                return new OperationResult { Result = result > 0 ? true : false, Error = string.Empty } ;
            }catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> UpdateAsync(AccountDTO account, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                if (account == null)
                    return new OperationResult { Result = false, Error = "The provided Account object is null." };

                Account? existingAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == account.UserId, token);
                if(existingAccount == null)
                    return new OperationResult { Result = false, Error = "Didn't find any account with the provided userId." };

                existingAccount.UserName = account.UserName ?? string.Empty;
                existingAccount.Email = account.Email ?? string.Empty;
                if (!string.IsNullOrEmpty(account.PasswordHashed))
                    existingAccount.PasswordHashed = BCrypt.Net.BCrypt.HashPassword(account.PasswordHashed);

                var result = await _context.SaveChangesAsync(token);
                return new OperationResult { Result = result > 0 ? true : false, Error = string.Empty };
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
                if (accountId == Guid.Empty)
                    return new OperationResult { Result = false, Error = "The provided AccountId is null." };

                var accountObject = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == accountId, token);
                if (accountObject == null)
                    return new OperationResult { Result = false, Error = "There is no settings with provided id." };

                _context.Accounts.Remove(accountObject);

                var result = await _context.SaveChangesAsync(token);
                return new OperationResult { Result = result > 0 ? true : false, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }
    }
}
