using AutoMapper;
using Calmska.Models.DTO;
using Calmska.Api.Helper;
using Calmska.Api.Interfaces;
using Calmska.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Calmska.Api.Repository
{
    public class MoodRepository : IRepository<Mood, MoodDTO>
    {
        private readonly CalmskaDbContext _context;
        private readonly IMapper _mapper;
        public MoodRepository(CalmskaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PaginatedResult<Mood>> GetAllAsync(int? pageNumber, int? pageSize)
        {
            var query = await Task.Run(_context.Moods.AsQueryable);
            return Pagination.Paginate(query, pageNumber, pageSize);
        }

        public async Task<PaginatedResult<Mood>> GetAllByArgumentAsync(MoodDTO moodDTO, int? pageNumber, int? pageSize)
        {
            var query = await Task.Run(_context.Moods
                .Where(item =>
                    (!moodDTO.MoodId.HasValue || item.MoodId == moodDTO.MoodId) &&
                    (string.IsNullOrEmpty(moodDTO.MoodName) || item.MoodName.ToLower().Contains(moodDTO.MoodName.ToLower())) &&
                    (string.IsNullOrEmpty(moodDTO.Type) || item.Type.ToLower().Contains(moodDTO.Type.ToLower()))
                )
                .AsQueryable);

            return Pagination.Paginate(query, pageNumber, pageSize);
        }

        public async Task<Mood?> GetByArgumentAsync(MoodDTO moodDTO)
        {
            return await _context.Moods
               .Where(item =>
                   (!moodDTO.MoodId.HasValue || item.MoodId == moodDTO.MoodId) &&
                   (string.IsNullOrEmpty(moodDTO.MoodName) || item.MoodName.ToLower().Contains(moodDTO.MoodName.ToLower())) &&
                   (string.IsNullOrEmpty(moodDTO.Type) || item.Type.ToLower().Contains(moodDTO.Type.ToLower()))
               )
               .FirstOrDefaultAsync();
        }

        public async Task<OperationResult> AddAsync(MoodDTO moodDTO)
        {
            try
            {
                if (moodDTO == null)
                    return new OperationResult { Result = false, Error = "The provided Mood object is null." };

                var mood = _mapper.Map<Mood>(moodDTO);
                await _context.Moods.AddAsync(mood);

                var result = await _context.SaveChangesAsync();
                return new OperationResult { Result = result > 0 ? true : false, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> UpdateAsync(MoodDTO moodDto)
        {
            try
            {
                if (moodDto == null)
                    return new OperationResult { Result = false, Error = "The provided Mood object is null." };

                Mood? existingMood = await _context.Moods.FirstOrDefaultAsync(m => m.MoodId == moodDto.MoodId);
                if (existingMood == null)
                    return new OperationResult { Result = false, Error = "Didn't find any mood with the provided moodId." };

                existingMood.MoodName = moodDto.MoodName ?? string.Empty;
                existingMood.Type = moodDto.Type ?? string.Empty;

                var result = await _context.SaveChangesAsync();
                return new OperationResult { Result = result > 0 ? true : false, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> DeleteAsync(Guid moodId)
        {
            try
            {
                if (moodId == Guid.Empty)
                    return new OperationResult { Result = false, Error = "The provided MoodId is null." };

                var moodObject = await _context.Moods.FirstOrDefaultAsync(a => a.MoodId == moodId);
                if (moodObject == null)
                    return new OperationResult { Result = false, Error = "There is no mood with provided id." };

                _context.Moods.Remove(moodObject);

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
