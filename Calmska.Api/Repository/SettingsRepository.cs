using Calmska.Api.DTO;
using Calmska.Api.Interfaces;
using Calmska.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Calmska.Api.Repository
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly CalmskaDbContext _context;
        public SettingsRepository(CalmskaDbContext context) => _context = context;
        public async Task<IEnumerable<Settings>> GetAllAsync()
        {
            return await _context.SettingsDb.ToListAsync();
        }

        public async Task<IEnumerable<Settings>> GetAllByArgumentAsync(Settings settings)
        {
            return await _context.SettingsDb
                .Where(item =>
                    (settings.SettingsId == Guid.Empty || item.SettingsId.ToString().Contains(settings.SettingsId.ToString(), StringComparison.OrdinalIgnoreCase)) &&
                    (settings.SettingsJson == string.Empty || item.SettingsJson.Contains(settings.SettingsJson, StringComparison.OrdinalIgnoreCase)) &&
                    (settings.UserId == Guid.Empty || item.UserId.ToString().Contains(settings.UserId.ToString(), StringComparison.OrdinalIgnoreCase))
                    ).ToListAsync();
        }

        public async Task<Settings?> GetByArgumentAsync(Settings settings)
        {
            return await _context.SettingsDb
                .Where(item =>
                    (settings.SettingsId == Guid.Empty || item.SettingsId.ToString().Contains(settings.SettingsId.ToString(), StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(settings.SettingsJson) || item.SettingsJson.Contains(settings.SettingsJson, StringComparison.OrdinalIgnoreCase)) &&
                    (settings.UserId == Guid.Empty || item.UserId.ToString().Contains(settings.UserId.ToString(), StringComparison.OrdinalIgnoreCase))
                    ).FirstOrDefaultAsync();
        }

        public async Task<OperationResult> AddAsync(Settings settings)
        {
            try
            {
                if (settings == null)
                    return new OperationResult { Result = false, Error = "The provided Settings object is null." };

                await _context.SettingsDb.AddAsync(settings);

                var result = await _context.SaveChangesAsync();
                return new OperationResult { Result = result > 0 ? true : false, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> UpdateAsync(Settings settings)
        {
            try
            {
                if (settings == null)
                    return new OperationResult { Result = false, Error = "The provided Settings object is null." };

                Settings? existingSettings = await _context.SettingsDb.FirstOrDefaultAsync(a => a.SettingsId == a.SettingsId);
                if (existingSettings == null)
                    return new OperationResult { Result = false, Error = "Didn't find any settings with the provided settingsId." };

                existingSettings.SettingsJson = settings.SettingsJson;
                existingSettings.UserId = settings.UserId;

                var result = await _context.SaveChangesAsync();
                return new OperationResult { Result = result > 0 ? true : false, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> DeleteAsync(Settings settings)
        {
            try
            {
                if (settings == null)
                    return new OperationResult { Result = false, Error = "The provided Settings object is null." };

                _context.SettingsDb.Remove(settings);

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
