using AutoMapper;
using Calmska.Api.DTO;
using Calmska.Api.Helper;
using Calmska.Api.Interfaces;
using Calmska.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Calmska.Api.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly CalmskaDbContext _context;
        private readonly IMapper _mapper;
        public AccountRepository(CalmskaDbContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _context.Accounts.ToListAsync();
        }

        public async Task<IEnumerable<Account>> GetAllByArgumentAsync(AccountDTO account)
        {
            return await _context.Accounts
                .Where(item =>
                    (!account.UserId.HasValue || item.UserId == account.UserId) &&
                    (string.IsNullOrEmpty(account.UserName) || item.UserName.ToLower().Contains(account.UserName.ToLower())) &&
                    (string.IsNullOrEmpty(account.Email) || item.Email.ToLower().Contains(account.Email.ToLower())) &&
                    (string.IsNullOrEmpty(account.PasswordHashed) || item.PasswordHashed.ToLower().Contains(account.PasswordHashed.ToLower()))
                )
                .ToListAsync();
        }

        public async Task<Account?> GetByArgumentAsync(AccountDTO account)
        {
            return await _context.Accounts
                .Where(item =>
                    (!account.UserId.HasValue || item.UserId == account.UserId) &&
                    (string.IsNullOrEmpty(account.UserName) || item.UserName.ToLower().Contains(account.UserName.ToLower())) &&
                    (string.IsNullOrEmpty(account.Email) || item.Email.ToLower().Contains(account.Email.ToLower())) &&
                    (string.IsNullOrEmpty(account.PasswordHashed) || item.PasswordHashed.ToLower().Contains(account.PasswordHashed.ToLower()))
                )
                .FirstOrDefaultAsync();
        }

        public async Task<OperationResult> AddAsync(AccountDTO accountDto)
        {
            try
            {
                if (accountDto == null)
                    return new OperationResult { Result = false, Error = "The provided Account object is null." };

                var userByEmail = GetByArgumentAsync(new AccountDTO { Email = accountDto.Email, PasswordHashed = string.Empty, UserName = string.Empty, UserId = Guid.Empty});
                if (userByEmail.Result != null)
                    return new OperationResult { Result = false, Error = "The user with provided email exists." };

                accountDto.PasswordHashed = HashPassword.SetHash(accountDto.PasswordHashed);
                var account = _mapper.Map<Account>(accountDto);
                await _context.Accounts.AddAsync(account);

                var result = await _context.SaveChangesAsync();
                return new OperationResult { Result = result > 0 ? true : false, Error = string.Empty } ;
            }catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> UpdateAsync(AccountDTO account)
        {
            try
            {
                if (account == null)
                    return new OperationResult { Result = false, Error = "The provided Account object is null." };

                Account? existingAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == account.UserId);
                if(existingAccount == null)
                    return new OperationResult { Result = false, Error = "Didn't find any account with the provided userId." };

                existingAccount.UserName = account.UserName;
                existingAccount.Email = account.Email;
                if (!string.IsNullOrEmpty(account.PasswordHashed))
                    existingAccount.PasswordHashed = BCrypt.Net.BCrypt.HashPassword(account.PasswordHashed);

                var result = await _context.SaveChangesAsync();
                return new OperationResult { Result = result > 0 ? true : false, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> DeleteAsync(Guid accountId)
        {
            try
            {
                if (accountId == Guid.Empty)
                    return new OperationResult { Result = false, Error = "The provided AccountId is null." };

                var accountObject = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == accountId);
                if (accountObject == null)
                    return new OperationResult { Result = false, Error = "There is no settings with provided id." };

                _context.Accounts.Remove(accountObject);

                var result = await _context.SaveChangesAsync();
                return new OperationResult { Result = result > 0 ? true : false, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }
    }
}
