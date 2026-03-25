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
    public class SettingsRepository : IRepository<Settings, SettingsFilter>
    {
        private readonly CalmskaDbContext _context;
        private readonly IMapper _mapper;
        
        private const int _defaultPageNumber = 1;
        public SettingsRepository(CalmskaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PaginatedResult<Settings>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await Task.Run(_context.SettingsDb.AsQueryable, token);
            
            var documentResult = Pagination.Paginate(query, pageNumber, pageSize);
            var domainItems = _mapper.Map<IEnumerable<Settings>>(documentResult.Items);
            
            return new PaginatedResult<Settings>(domainItems, documentResult.TotalCount, pageNumber ?? _defaultPageNumber, pageSize ?? query.Count());
        }

        public async Task<PaginatedResult<Settings>> GetAllByArgumentAsync(SettingsFilter settings, int? pageNumber, int? pageSize,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await Task.Run(_context.SettingsDb
                .Where(item =>
                    (!settings.SettingsId.HasValue || item.SettingsId == settings.SettingsId) &&
                    (string.IsNullOrEmpty(settings.Color) || (item.Color != null && item.Color.ToLower().Contains(settings.Color.ToLower())) ) &&
                    (string.IsNullOrEmpty(settings.PomodoroTimer) || (item.PomodoroTimer != null && item.PomodoroTimer.ToLower().Contains(settings.PomodoroTimer.ToLower())) ) &&
                    (string.IsNullOrEmpty(settings.PomodoroBreak) || (item.PomodoroBreak != null && item.PomodoroBreak.ToLower().Contains(settings.PomodoroBreak.ToLower())) ) &&
                    (!settings.UserId.HasValue || item.UserId == settings.UserId)
                )
                .AsQueryable, token);

            var documentResult = Pagination.Paginate(query, pageNumber, pageSize);
            var domainItems = _mapper.Map<IEnumerable<Settings>>(documentResult.Items);
            
            return new PaginatedResult<Settings>(domainItems, documentResult.TotalCount, pageNumber ?? _defaultPageNumber, pageSize ?? query.Count());
        }

        public async Task<Settings?> GetByArgumentAsync(SettingsFilter settings, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await _context.SettingsDb
                .Where(item =>
                    (!settings.SettingsId.HasValue || item.SettingsId == settings.SettingsId) &&
                    (string.IsNullOrEmpty(settings.Color) || (item.Color != null && item.Color.ToLower().Contains(settings.Color.ToLower())) ) &&
                    (string.IsNullOrEmpty(settings.PomodoroTimer) || (item.PomodoroTimer != null && item.PomodoroTimer.ToLower().Contains(settings.PomodoroTimer.ToLower())) ) &&
                    (string.IsNullOrEmpty(settings.PomodoroBreak) || (item.PomodoroBreak != null && item.PomodoroBreak.ToLower().Contains(settings.PomodoroBreak.ToLower())) ) &&
                    (!settings.UserId.HasValue || item.UserId == settings.UserId)
                )
                .FirstOrDefaultAsync(token);
            
            return _mapper.Map<Settings?>(query);
        }

        public async Task<OperationResult> AddAsync(Settings settings, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                
                var document = _mapper.Map<SettingsDocument>(settings);
                await _context.SettingsDb.AddAsync(document, token);

                await _context.SaveChangesAsync(token);
                return new OperationResult { Result = true, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> UpdateAsync(SettingsFilter filter, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var existingSettings = await _context.SettingsDb.FirstOrDefaultAsync(a => a.SettingsId == filter.SettingsId, token);
                if (existingSettings == null)
                    return new OperationResult { Result = false, Error = "Didn't find any settings with the provided settingsId." };

                UpdateSettings(existingSettings, filter);

                await _context.SaveChangesAsync(token);
                return new OperationResult { Result = true, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> DeleteAsync(Guid settingsId, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var settingsObject = await _context.SettingsDb.FirstOrDefaultAsync(s => s.SettingsId == settingsId, token);
                if(settingsObject == null)
                    return new OperationResult { Result = false, Error = "There is no settings with provided id." };
                
                _context.SettingsDb.Remove(settingsObject);

                await _context.SaveChangesAsync(token);
                return new OperationResult { Result = true, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        private void UpdateSettings(SettingsDocument existingSettings, SettingsFilter filter)
        {
            if(!string.IsNullOrEmpty(filter.Color))
                existingSettings.Color = filter.Color;
            if(!string.IsNullOrEmpty(filter.PomodoroTimer))
                existingSettings.PomodoroTimer = filter.PomodoroTimer;
            if (!string.IsNullOrEmpty(filter.PomodoroBreak))
                existingSettings.PomodoroBreak = filter.PomodoroBreak;
            if (filter.UserId != null)
                existingSettings.UserId = (Guid)filter.UserId;
        }
    }
}
