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
    public class MoodHistoryRepository : IRepository<MoodHistory, MoodHistoryFilter>
    {
        private readonly CalmskaDbContext _context;
        private readonly IMapper _mapper;
        
        public MoodHistoryRepository(CalmskaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PaginatedResult<MoodHistory>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await Task.Run(_context.MoodHistoryDb.AsQueryable, token);
            
            var documentResult = Pagination.Paginate(query, pageNumber, pageSize);
            var domainItems = _mapper.Map<IEnumerable<MoodHistory>>(documentResult.Items);
            
            return new PaginatedResult<MoodHistory>(domainItems, documentResult.TotalCount, documentResult.PageNumber, documentResult.PageSize);
        }

        public async Task<PaginatedResult<MoodHistory>> GetAllByArgumentAsync(MoodHistoryFilter filter, int? pageNumber, int? pageSize,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await Task.Run(_context.MoodHistoryDb
                .Where(item =>
                    (!filter.MoodHistoryId.HasValue || item.MoodHistoryId == filter.MoodHistoryId) &&
                    (!filter.Date.HasValue || item.Date.Date == filter.Date.Value.Date) &&
                    (!filter.UserId.HasValue || item.UserId == filter.UserId) &&
                    (!filter.MoodId.HasValue || item.MoodId == filter.MoodId)
                )
                .AsQueryable, token);

            var documentResult = Pagination.Paginate(query, pageNumber, pageSize);
            var domainItems = _mapper.Map<IEnumerable<MoodHistory>>(documentResult.Items);
            
            return new PaginatedResult<MoodHistory>(domainItems, documentResult.TotalCount, documentResult.PageNumber, documentResult.PageSize);
        }

        public async Task<MoodHistory?> GetByArgumentAsync(MoodHistoryFilter filter, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await _context.MoodHistoryDb
                .Where(item =>
                    (!filter.MoodHistoryId.HasValue || item.MoodHistoryId == filter.MoodHistoryId) &&
                    (!filter.Date.HasValue || item.Date.Date == filter.Date.Value.Date) &&
                    (!filter.UserId.HasValue || item.UserId == filter.UserId) &&
                    (!filter.MoodId.HasValue || item.MoodId == filter.MoodId)
                )
                .FirstOrDefaultAsync(token);
            
            return _mapper.Map<MoodHistory?>(query);
        }

        public async Task<OperationResult> AddAsync(MoodHistory moodHistory, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var document = _mapper.Map<MoodHistoryDocument>(moodHistory);
                await _context.MoodHistoryDb.AddAsync(document, token);

                await _context.SaveChangesAsync(token);
                return new OperationResult { Result = true, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> UpdateAsync(MoodHistoryFilter filter, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var existingMoodHistory = await _context.MoodHistoryDb.FirstOrDefaultAsync(a => a.MoodHistoryId == filter.MoodHistoryId, token);
                if (existingMoodHistory == null)
                    return new OperationResult { Result = false, Error = "Didn't find any mood with the provided moodId." };

                UpdateMoodHistory(existingMoodHistory, filter);

                await _context.SaveChangesAsync(token);
                return new OperationResult { Result = true, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> DeleteAsync(Guid moodHistoryId, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var moodHistoryObject = await _context.MoodHistoryDb.FirstOrDefaultAsync(a => a.MoodHistoryId == moodHistoryId, token);
                if (moodHistoryObject == null)
                    return new OperationResult { Result = false, Error = "There is no moodHistory with provided id." };

                _context.MoodHistoryDb.Remove(moodHistoryObject);

                await _context.SaveChangesAsync(token);
                return new OperationResult { Result = true, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        private void UpdateMoodHistory(MoodHistoryDocument existingMoodHistory, MoodHistoryFilter filter)
        {
            if(filter.UserId is not null && filter.UserId != Guid.Empty)
                existingMoodHistory.UserId = (Guid)filter.UserId;
            if(filter.MoodId is not null && filter.MoodId != Guid.Empty)
                existingMoodHistory.MoodId = (Guid)filter.MoodId;
                    
            existingMoodHistory.Date = DateTime.UtcNow;
        }
    }
}
