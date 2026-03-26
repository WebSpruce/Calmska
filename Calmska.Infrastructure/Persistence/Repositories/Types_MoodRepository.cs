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
    public class Types_MoodRepository : ITypesRepository<Types_Mood, Types_MoodFilter>
    {
        private readonly CalmskaDbContext _context;
        private readonly IMapper _mapper;
        
        public Types_MoodRepository(CalmskaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PaginatedResult<Types_Mood>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await Task.Run(_context.Types_MoodDb.AsQueryable, token);
            
            var documentResult = Pagination.Paginate(query, pageNumber, pageSize);
            var domainItems = _mapper.Map<IEnumerable<Types_Mood>>(documentResult.Items);
            
            return new PaginatedResult<Types_Mood>(domainItems, documentResult.TotalCount, documentResult.PageNumber, documentResult.PageSize);
        }

        public async Task<PaginatedResult<Types_Mood>> GetAllByArgumentAsync(Types_MoodFilter moods, int? pageNumber, int? pageSize,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await Task.Run(_context.Types_MoodDb
                .Where(item =>
                    (!moods.TypeId.HasValue || item.TypeId == moods.TypeId) &&
                    (string.IsNullOrEmpty(moods.Type) || item.Type.ToLower().Contains(moods.Type.ToLower()))
                )
                .AsQueryable, token);

            var documentResult = Pagination.Paginate(query, pageNumber, pageSize);
            var domainItems = _mapper.Map<IEnumerable<Types_Mood>>(documentResult.Items);
            
            return new PaginatedResult<Types_Mood>(domainItems, documentResult.TotalCount, documentResult.PageNumber, documentResult.PageSize);
        }

        public async Task<Types_Mood?> GetByArgumentAsync(Types_MoodFilter moods, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await _context.Types_MoodDb
                .Where(item =>
                    (!moods.TypeId.HasValue || item.TypeId == moods.TypeId) &&
                    (string.IsNullOrEmpty(moods.Type) || item.Type.ToLower().Contains(moods.Type.ToLower()))
                )
                .FirstOrDefaultAsync(token);
            
            return _mapper.Map<Types_Mood?>(query);
        }

        public async Task<OperationResult> AddAsync(Types_Mood typesMood, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var document = _mapper.Map<Types_MoodDocument>(typesMood);
                await _context.Types_MoodDb.AddAsync(document, token);

                await _context.SaveChangesAsync(token);
                return new OperationResult { Result = true, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> UpdateAsync(Types_MoodFilter moods, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var existingTypemood = await _context.Types_MoodDb.FirstOrDefaultAsync(t => t.TypeId == moods.TypeId, token);
                if (existingTypemood == null)
                    return new OperationResult { Result = false, Error = "Didn't find any type_mood with the provided type_moodsId." };

                existingTypemood.Type = moods.Type ?? string.Empty;

                await _context.SaveChangesAsync(token);
                return new OperationResult { Result = true, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> DeleteAsync(int type_moodId, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                if (type_moodId <= 0)
                    return new OperationResult { Result = false, Error = "The provided type_moodId is <= 0." };

                var typeObject = await _context.Types_MoodDb.FirstOrDefaultAsync(t => t.TypeId == type_moodId, token);
                if (typeObject == null)
                    return new OperationResult { Result = false, Error = "There is no type_mood with provided id." };

                _context.Types_MoodDb.Remove(typeObject);

                await _context.SaveChangesAsync(token);
                return new OperationResult { Result = true, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }
    }
}
