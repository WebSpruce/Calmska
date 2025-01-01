using AutoMapper;
using Calmska.Models.DTO;
using Calmska.Api.Helper;
using Calmska.Api.Interfaces;
using Calmska.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Calmska.Api.Repository
{
    public class MoodHistoryRepository : IRepository<MoodHistory, MoodHistoryDTO>
    {
        private readonly CalmskaDbContext _context;
        private readonly IMapper _mapper;
        public MoodHistoryRepository(CalmskaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PaginatedResult<MoodHistory>> GetAllAsync(int? pageNumber, int? pageSize)
        {
            var query = await Task.Run(_context.MoodHistoryDb.AsQueryable);
            return Pagination.Paginate(query, pageNumber, pageSize);
        }

        public async Task<PaginatedResult<MoodHistory>> GetAllByArgumentAsync(MoodHistoryDTO moodHistoryDTO, int? pageNumber, int? pageSize)
        {
            var query = await Task.Run(_context.MoodHistoryDb
                .Where(item =>
                    (moodHistoryDTO.MoodHistoryId == Guid.Empty || !moodHistoryDTO.MoodHistoryId.HasValue || item.MoodHistoryId == moodHistoryDTO.MoodHistoryId) &&
                    (!moodHistoryDTO.Date.HasValue || item.Date.Date == moodHistoryDTO.Date.Value.Date) &&
                    (moodHistoryDTO.UserId == Guid.Empty || !moodHistoryDTO.UserId.HasValue || item.UserId == moodHistoryDTO.UserId) &&
                    (moodHistoryDTO.MoodId == Guid.Empty || !moodHistoryDTO.MoodId.HasValue || item.MoodId == moodHistoryDTO.MoodId)
                )
                .AsQueryable);

            return Pagination.Paginate(query, pageNumber, pageSize);
        }

        public async Task<MoodHistory?> GetByArgumentAsync(MoodHistoryDTO moodHistoryDTO)
        {
            return await _context.MoodHistoryDb
                .Where(item =>
                    (moodHistoryDTO.MoodHistoryId == Guid.Empty || !moodHistoryDTO.MoodHistoryId.HasValue || item.MoodHistoryId == moodHistoryDTO.MoodHistoryId) &&
                    (!moodHistoryDTO.Date.HasValue || item.Date.Date == moodHistoryDTO.Date.Value.Date) &&
                    (moodHistoryDTO.UserId == Guid.Empty || !moodHistoryDTO.UserId.HasValue || item.UserId == moodHistoryDTO.UserId) &&
                    (moodHistoryDTO.MoodId == Guid.Empty || !moodHistoryDTO.MoodId.HasValue || item.MoodId == moodHistoryDTO.MoodId)
                )
                .FirstOrDefaultAsync();
        }

        public async Task<OperationResult> AddAsync(MoodHistoryDTO moodHistoryDTO)
        {
            try
            {
                if (moodHistoryDTO == null)
                    return new OperationResult { Result = false, Error = "The provided MoodHistory object is null." };

                var userByEmail = GetByArgumentAsync(new MoodHistoryDTO { UserId = moodHistoryDTO.UserId, MoodId = Guid.Empty, Date = null, MoodHistoryId = Guid.Empty });
                if (userByEmail.Result != null)
                    return new OperationResult { Result = false, Error = "The moodHistory for provided UserId already exists." };

                moodHistoryDTO.Date = DateTime.UtcNow;

                var moodHistory = _mapper.Map<MoodHistory>(moodHistoryDTO);
                await _context.MoodHistoryDb.AddAsync(moodHistory);

                var result = await _context.SaveChangesAsync();
                return new OperationResult { Result = result > 0 ? true : false, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> UpdateAsync(MoodHistoryDTO moodHistoryDTO)
        {
            try
            {
                if (moodHistoryDTO == null)
                    return new OperationResult { Result = false, Error = "The provided MoodHistory object is null." };

                MoodHistory? existingMoodHistory = await _context.MoodHistoryDb.FirstOrDefaultAsync(a => a.MoodHistoryId == moodHistoryDTO.MoodHistoryId);
                if (existingMoodHistory == null)
                    return new OperationResult { Result = false, Error = "Didn't find any account with the provided userId." };

                existingMoodHistory.Date = DateTime.UtcNow;
                existingMoodHistory.UserId = moodHistoryDTO.UserId ?? Guid.Empty;
                existingMoodHistory.MoodId = moodHistoryDTO.MoodId ?? Guid.Empty;

                var result = await _context.SaveChangesAsync();
                return new OperationResult { Result = result > 0 ? true : false, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> DeleteAsync(Guid moodHistoryId)
        {
            try
            {
                if (moodHistoryId == Guid.Empty)
                    return new OperationResult { Result = false, Error = "The provided MoodHistoryId is null." };

                var moodHistoryObject = await _context.MoodHistoryDb.FirstOrDefaultAsync(a => a.MoodHistoryId == moodHistoryId);
                if (moodHistoryObject == null)
                    return new OperationResult { Result = false, Error = "There is no moodHistory with provided id." };

                _context.MoodHistoryDb.Remove(moodHistoryObject);

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
