using AutoMapper;
using Calmska.Models.DTO;
using Calmska.Api.Helper;
using Calmska.Api.Interfaces;
using Calmska.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Calmska.Api.Repository
{
    public class Types_MoodRepository : ITypesRepository<Types_Mood, Types_MoodDTO>
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
            return Pagination.Paginate(query, pageNumber, pageSize);
        }

        public async Task<PaginatedResult<Types_Mood>> GetAllByArgumentAsync(Types_MoodDTO moods, int? pageNumber, int? pageSize,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await Task.Run(_context.Types_MoodDb
                .Where(item =>
                    (!moods.TypeId.HasValue || item.TypeId == moods.TypeId) &&
                    (string.IsNullOrEmpty(moods.Type) || item.Type.ToLower().Contains(moods.Type.ToLower()))
                )
                .AsQueryable, token);

            return Pagination.Paginate(query, pageNumber, pageSize);
        }

        public async Task<Types_Mood?> GetByArgumentAsync(Types_MoodDTO moods, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return await _context.Types_MoodDb
                .Where(item =>
                    (!moods.TypeId.HasValue || item.TypeId == moods.TypeId) &&
                    (string.IsNullOrEmpty(moods.Type) || item.Type.ToLower().Contains(moods.Type.ToLower()))
                )
                .FirstOrDefaultAsync(token);
        }

        public async Task<OperationResult> AddAsync(Types_MoodDTO Types_MoodDTO, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                if (Types_MoodDTO == null)
                    return new OperationResult { Result = false, Error = "The provided Type_mood object is null." };

                var typemood = _mapper.Map<Types_Mood>(Types_MoodDTO);
                await _context.Types_MoodDb.AddAsync(typemood, token);

                var result = await _context.SaveChangesAsync(token);
                return new OperationResult { Result = result > 0 ? true : false, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> UpdateAsync(Types_MoodDTO moods, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                if (moods == null)
                    return new OperationResult { Result = false, Error = "The provided Type_mood object is null." };

                Types_Mood? existingTypemood = await _context.Types_MoodDb.FirstOrDefaultAsync(t => t.TypeId == moods.TypeId, token);
                if (existingTypemood == null)
                    return new OperationResult { Result = false, Error = "Didn't find any type_mood with the provided type_moodsId." };

                existingTypemood.Type = moods.Type ?? string.Empty;

                var result = await _context.SaveChangesAsync(token);
                return new OperationResult { Result = result > 0 ? true : false, Error = string.Empty };
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

                var result = await _context.SaveChangesAsync(token);
                return new OperationResult { Result = result > 0 ? true : false, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }
    }
}
