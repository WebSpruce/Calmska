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
    public class MoodRepository : IRepository<Mood, MoodFilter>
    {
        private readonly CalmskaDbContext _context;
        private readonly IMapper _mapper;
        
        public MoodRepository(CalmskaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PaginatedResult<Mood>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await Task.Run(_context.Moods.AsQueryable, token);
            
            var documentResult = Pagination.Paginate(query, pageNumber, pageSize);
            var domainItems = _mapper.Map<IEnumerable<Mood>>(documentResult.Items);

            return new PaginatedResult<Mood>(domainItems, documentResult.TotalCount, documentResult.PageNumber, documentResult.PageSize);
        }

        public async Task<PaginatedResult<Mood>> GetAllByArgumentAsync(MoodFilter filter, int? pageNumber, int? pageSize,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await Task.Run(_context.Moods
                .Where(item =>
                    (!filter.MoodId.HasValue || item.MoodId == filter.MoodId) &&
                    (string.IsNullOrEmpty(filter.MoodName) || (item.MoodName != null && item.MoodName.ToLower().Contains(filter.MoodName.ToLower())) ) &&
                    (filter.MoodTypeId == null || filter.MoodTypeId == 0 || item.MoodTypeId == filter.MoodTypeId)
                )
                .AsQueryable, token);

            var documentResult = Pagination.Paginate(query, pageNumber, pageSize);
            var domainItems = _mapper.Map<IEnumerable<Mood>>(documentResult.Items);

            return new PaginatedResult<Mood>(domainItems, documentResult.TotalCount, documentResult.PageNumber, documentResult.PageSize);
        }

        public async Task<Mood?> GetByArgumentAsync(MoodFilter filter, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await _context.Moods
               .Where(item =>
                   (!filter.MoodId.HasValue || item.MoodId.ToString() == filter.MoodId.ToString()) &&
                   (string.IsNullOrEmpty(filter.MoodName) || (item.MoodName != null && item.MoodName.ToLower().Contains(filter.MoodName.ToLower())) ) &&
                   (filter.MoodTypeId == null || filter.MoodTypeId == 0 || item.MoodTypeId == filter.MoodTypeId)
               )
               .FirstOrDefaultAsync(token);
            
            return _mapper.Map<Mood?>(query);
        }

        public async Task<OperationResult> AddAsync(Mood mood, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var moodDocument = _mapper.Map<MoodDocument>(mood);
                await _context.Moods.AddAsync(moodDocument, token);

                await _context.SaveChangesAsync(token);
                return new OperationResult { Result = true, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> UpdateAsync(MoodFilter filter, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var existingMood = await _context.Moods.FirstOrDefaultAsync(m => m.MoodId == filter.MoodId, token);
                if (existingMood == null)
                    return new OperationResult { Result = false, Error = "Didn't find any mood with the provided Id." };

                UpdateMood(existingMood, filter);

                await _context.SaveChangesAsync(token);
                return new OperationResult { Result = true, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> DeleteAsync(Guid moodId, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var moodObject = await _context.Moods.FirstOrDefaultAsync(a => a.MoodId == moodId, token);
                if (moodObject == null)
                    return new OperationResult { Result = false, Error = "There is no mood with provided id." };

                _context.Moods.Remove(moodObject);

                await _context.SaveChangesAsync(token);
                return new OperationResult { Result = true, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        private void UpdateMood(MoodDocument existingMood, MoodFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.MoodName))
                existingMood.MoodName = filter.MoodName;
            if(filter.MoodTypeId is not null)
                existingMood.MoodTypeId = (int)filter.MoodTypeId;
        }
    }
}
