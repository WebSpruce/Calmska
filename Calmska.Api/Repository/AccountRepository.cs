using Calmska.Api.Interfaces;
using Calmska.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Calmska.Api.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly CalmskaDbContext _context;
        public AccountRepository(CalmskaDbContext context) => _context = context;

        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _context.Accounts.ToListAsync();
        }

        public async Task<IEnumerable<Account>> GetAllByArgumentAsync(Account account)
        {
            return await _context.Accounts
                .Where(item => (!string.IsNullOrEmpty(account.UserId.ToString()) && item.UserId.ToString().Contains(account.UserId.ToString(), StringComparison.OrdinalIgnoreCase))
                                && (!string.IsNullOrEmpty(account.UserName) && item.UserName.Contains(account.UserName, StringComparison.OrdinalIgnoreCase))
                                && (!string.IsNullOrEmpty(account.Email) && item.Email.Contains(account.Email, StringComparison.OrdinalIgnoreCase))
                                && (!string.IsNullOrEmpty(account.PasswordHashed) && item.PasswordHashed.Contains(account.PasswordHashed, StringComparison.OrdinalIgnoreCase)))
                .ToListAsync();
        }

        public async Task<Account?> GetByArgumentAsync(Account account)
        {
            return _context.Accounts != null ? await _context.Accounts
                .Where(item => (!string.IsNullOrEmpty(account.UserId.ToString()) && item.UserId.ToString().Contains(account.UserId.ToString(), StringComparison.OrdinalIgnoreCase))
                                && (!string.IsNullOrEmpty(account.UserName) && item.UserName.Contains(account.UserName, StringComparison.OrdinalIgnoreCase))
                                && (!string.IsNullOrEmpty(account.Email) && item.Email.Contains(account.Email, StringComparison.OrdinalIgnoreCase))
                                && (!string.IsNullOrEmpty(account.PasswordHashed) && item.PasswordHashed.Contains(account.PasswordHashed, StringComparison.OrdinalIgnoreCase)))
                .FirstOrDefaultAsync() : null;
        }

        public Task<bool> AddAsync(Account account)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Account account)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Account account)
        {
            throw new NotImplementedException();
        }
    }
}
