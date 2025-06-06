﻿using AutoMapper;
using Calmska.Models.DTO;
using Calmska.Api.Helper;
using Calmska.Api.Interfaces;
using Calmska.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Calmska.Api.Repository
{
    public class SettingsRepository : IRepository<Settings, SettingsDTO>
    {
        private readonly CalmskaDbContext _context;
        private readonly IMapper _mapper;
        public SettingsRepository(CalmskaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PaginatedResult<Settings>> GetAllAsync(int? pageNumber, int? pageSize)
        {
            var query = await Task.Run(_context.SettingsDb.AsQueryable);
            return Pagination.Paginate(query, pageNumber, pageSize);
        }

        public async Task<PaginatedResult<Settings>> GetAllByArgumentAsync(SettingsDTO settings, int? pageNumber, int? pageSize)
        {
            var query = await Task.Run(_context.SettingsDb
                .Where(item =>
                    (!settings.SettingsId.HasValue || item.SettingsId == settings.SettingsId) &&
                    (string.IsNullOrEmpty(settings.Color) || item.Color.ToLower().Contains(settings.Color.ToLower())) &&
                    (string.IsNullOrEmpty(settings.PomodoroTimer) || item.PomodoroTimer.ToLower().Contains(settings.PomodoroTimer.ToLower())) &&
                    (string.IsNullOrEmpty(settings.PomodoroBreak) || item.PomodoroBreak.ToLower().Contains(settings.PomodoroBreak.ToLower())) &&
                    (!settings.UserId.HasValue || item.UserId == settings.UserId)
                )
                .AsQueryable);

            return Pagination.Paginate(query, pageNumber, pageSize);
        }

        public async Task<Settings?> GetByArgumentAsync(SettingsDTO settings)
        {
            return await _context.SettingsDb
                .Where(item =>
                    (!settings.SettingsId.HasValue || item.SettingsId == settings.SettingsId) &&
                    (string.IsNullOrEmpty(settings.Color) || item.Color.ToLower().Contains(settings.Color.ToLower())) &&
                    (string.IsNullOrEmpty(settings.PomodoroTimer) || item.PomodoroTimer.ToLower().Contains(settings.PomodoroTimer.ToLower())) &&
                    (string.IsNullOrEmpty(settings.PomodoroBreak) || item.PomodoroBreak.ToLower().Contains(settings.PomodoroBreak.ToLower())) &&
                    (!settings.UserId.HasValue || item.UserId == settings.UserId)
                )
                .FirstOrDefaultAsync();
        }

        public async Task<OperationResult> AddAsync(SettingsDTO settingsDTO)
        {
            try
            {
                if (settingsDTO == null)
                    return new OperationResult { Result = false, Error = "The provided Settings object is null." };

                var settingsByUserId = GetByArgumentAsync(new SettingsDTO { UserId = settingsDTO.UserId, SettingsId = Guid.Empty });
                if (settingsByUserId.Result != null)
                    return new OperationResult { Result = false, Error = $"The settings object exists for the user with id: {settingsDTO.UserId}." };

                var settings = _mapper.Map<Settings>(settingsDTO);
                await _context.SettingsDb.AddAsync(settings);

                var result = await _context.SaveChangesAsync();
                return new OperationResult { Result = result > 0 ? true : false, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> UpdateAsync(SettingsDTO settingsDTO)
        {
            try
            {
                if (settingsDTO == null)
                    return new OperationResult { Result = false, Error = "The provided Settings object is null." };

                Settings? existingSettings = await _context.SettingsDb.FirstOrDefaultAsync(a => a.SettingsId == settingsDTO.SettingsId);
                if (existingSettings == null)
                    return new OperationResult { Result = false, Error = "Didn't find any settings with the provided settingsId." };

                if(!string.IsNullOrEmpty(settingsDTO.Color))
                    existingSettings.Color = settingsDTO.Color;
                if(!string.IsNullOrEmpty(settingsDTO.PomodoroTimer.ToString()))
                    existingSettings.PomodoroTimer = settingsDTO.PomodoroTimer.ToString() ?? string.Empty;
                if (!string.IsNullOrEmpty(settingsDTO.PomodoroBreak.ToString()))
                    existingSettings.PomodoroBreak = settingsDTO.PomodoroBreak.ToString() ?? string.Empty;


                if (settingsDTO.UserId != null)
                    existingSettings.UserId = (Guid)settingsDTO.UserId;

                var result = await _context.SaveChangesAsync();
                return new OperationResult { Result = result > 0 ? true : false, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> DeleteAsync(Guid settingsId)
        {
            try
            {
                if (settingsId == Guid.Empty)
                    return new OperationResult { Result = false, Error = "The provided SettingsId is null." };

                var settingsObject = await _context.SettingsDb.FirstOrDefaultAsync(s => s.SettingsId == settingsId);
                if(settingsObject == null)
                    return new OperationResult { Result = false, Error = "There is no settings with provided id." };
                
                _context.SettingsDb.Remove(settingsObject);

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
