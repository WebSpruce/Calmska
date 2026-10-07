using System.Globalization;
using AutoMapper;
using Calmska.Application.DTO;
using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using Calmska.Infrastructure.Persistence.Models;
using Calmska.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace Calmska.Infrastructure.Persistence.Repositories
{
    public class SettingsRepository : ISettingsRepository<Settings, SettingsDTO, SettingsFilter>
    {
        private readonly CalmskaDbContext _context;
        private readonly IMapper _mapper;
        
        public SettingsRepository(CalmskaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PaginatedResult<Settings>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = _context.SettingsDb.AsQueryable();
            
            var documentResult = await Pagination.Paginate(query, pageNumber, pageSize);
            var domainItems = _mapper.Map<IEnumerable<Settings>>(documentResult.Items);
            
            return new PaginatedResult<Settings>(domainItems, documentResult.TotalCount, documentResult.PageNumber, documentResult.PageSize);
        }

        public async Task<PaginatedResult<Settings>> GetAllByArgumentAsync(SettingsFilter settings, int? pageNumber, int? pageSize,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            
            var query = _context.SettingsDb.AsNoTracking();

            if (settings.SettingsId.HasValue)
                query = query.Where(item => item.SettingsId == settings.SettingsId.Value);
        
            if (settings.UserId.HasValue)
                query = query.Where(item => item.UserId == settings.UserId.Value);
        
            if (!string.IsNullOrWhiteSpace(settings.Color))
                query = query.Where(item => item.Color != null && item.Color.Contains(settings.Color));
        
            if (settings.PomodoroTimer.HasValue)
            {
                string timerStr = settings.PomodoroTimer.Value.ToString(CultureInfo.InvariantCulture);
                query = query.Where(item => item.PomodoroTimer != null && item.PomodoroTimer.Contains(timerStr));
            }
        
            if (settings.PomodoroBreak.HasValue)
            {
                string breakStr = settings.PomodoroBreak.Value.ToString(CultureInfo.InvariantCulture);
                query = query.Where(item => item.PomodoroBreak != null && item.PomodoroBreak.Contains(breakStr));
            }
        
            int totalCount = await query.CountAsync(token);
        
            int resolvedPageNumber = pageNumber ?? 1;
            int resolvedPageSize = pageSize ?? 10;
        
            var pagedEntities = await query
                .Skip((resolvedPageNumber - 1) * resolvedPageSize)
                .Take(resolvedPageSize)
                .ToListAsync(token);
        
            var domainItems = _mapper.Map<IEnumerable<Settings>>(pagedEntities);
            
            return new PaginatedResult<Settings>(
                domainItems, 
                totalCount, 
                resolvedPageNumber, 
                resolvedPageSize
            );
        }

        public async Task<SettingsDTO?> GetByArgumentAsync(SettingsFilter settings, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var query = _context.SettingsDb.AsNoTracking();

            if (settings.SettingsId.HasValue)
                query = query.Where(item => item.SettingsId == settings.SettingsId.Value);

            if (settings.UserId.HasValue)
                query = query.Where(item => item.UserId == settings.UserId.Value);

            if (!string.IsNullOrWhiteSpace(settings.Color))
                query = query.Where(item => item.Color != null && 
                                            item.Color.Contains(settings.Color)); 

            if (settings.PomodoroTimer.HasValue) 
            {
                string timerStr = settings.PomodoroTimer.Value.ToString(CultureInfo.InvariantCulture);
        
                query = query.Where(item => item.PomodoroTimer != null && 
                                            item.PomodoroTimer.Contains(timerStr));
            }

            if (settings.PomodoroBreak.HasValue)
            {
                string breakStr = settings.PomodoroBreak.Value.ToString(CultureInfo.InvariantCulture);
        
                query = query.Where(item => item.PomodoroBreak != null && 
                                            item.PomodoroBreak.Contains(breakStr));
            }

            var entity = await query.FirstOrDefaultAsync(token); 

            return _mapper.Map<SettingsDTO?>(entity);
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
            if(existingSettings.PomodoroTimer != filter.PomodoroTimer.ToString())
                existingSettings.PomodoroTimer = filter.PomodoroTimer.ToString();
            if (existingSettings.PomodoroBreak != filter.PomodoroBreak.ToString())
                existingSettings.PomodoroBreak = filter.PomodoroBreak.ToString();
            if (filter.UserId != null)
                existingSettings.UserId = (Guid)filter.UserId;
        }
    }
}
