using AutoMapper;
using Calmska.Models.DTO;
using Calmska.Api.Helper;
using Calmska.Api.Interfaces;
using Calmska.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Calmska.Api.Repository
{
    public class Types_TipsRepository : ITypesRepository<Types_Tips, Types_TipsDTO>
    {
        private readonly CalmskaDbContext _context;
        private readonly IMapper _mapper;
        public Types_TipsRepository(CalmskaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PaginatedResult<Types_Tips>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await Task.Run(_context.Types_TipsDb.AsQueryable, token);
            return Pagination.Paginate(query, pageNumber, pageSize);
        }

        public async Task<PaginatedResult<Types_Tips>> GetAllByArgumentAsync(Types_TipsDTO tips, int? pageNumber, int? pageSize,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await Task.Run(_context.Types_TipsDb
                .Where(item =>
                    (!tips.TypeId.HasValue || item.TypeId == tips.TypeId) &&
                    (string.IsNullOrEmpty(tips.Type) || item.Type.ToLower().Contains(tips.Type.ToLower()))
                )
                .AsQueryable, token);

            return Pagination.Paginate(query, pageNumber, pageSize);
        }

        public async Task<Types_Tips?> GetByArgumentAsync(Types_TipsDTO tips, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return await _context.Types_TipsDb
                .Where(item =>
                    (!tips.TypeId.HasValue || item.TypeId == tips.TypeId) &&
                    (string.IsNullOrEmpty(tips.Type) || item.Type.ToLower().Contains(tips.Type.ToLower()))
                )
                .FirstOrDefaultAsync(token);
        }

        public async Task<OperationResult> AddAsync(Types_TipsDTO Types_TipsDTO, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                if (Types_TipsDTO == null)
                    return new OperationResult { Result = false, Error = "The provided Type_Tip object is null." };

                int maxTypeId = 0;
                if (await _context.Types_TipsDb.AnyAsync(token))
                {
                    maxTypeId = _context.Types_TipsDb.ToList().MaxBy(t => t.TypeId).TypeId;
                }

                var typeTip = _mapper.Map<Types_Tips>(Types_TipsDTO);
                typeTip.TypeId = maxTypeId + 1;
                await _context.Types_TipsDb.AddAsync(typeTip, token);

                var result = await _context.SaveChangesAsync(token);
                return new OperationResult { Result = result > 0 ? true : false, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> UpdateAsync(Types_TipsDTO tips, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                if (tips == null)
                    return new OperationResult { Result = false, Error = "The provided Type_Tip object is null." };

                Types_Tips? existingTypeTip = await _context.Types_TipsDb.FirstOrDefaultAsync(t => t.TypeId == tips.TypeId, token);
                if (existingTypeTip == null)
                    return new OperationResult { Result = false, Error = "Didn't find any type_tip with the provided type_tipsId." };

                existingTypeTip.Type = tips.Type ?? string.Empty;

                var result = await _context.SaveChangesAsync(token);
                return new OperationResult { Result = result > 0 ? true : false, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> DeleteAsync(int type_tipId, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                if (type_tipId <= 0)
                    return new OperationResult { Result = false, Error = "The provided type_tipId is <= 0." };

                var typeObject = await _context.Types_TipsDb.FirstOrDefaultAsync(t => t.TypeId == type_tipId, token);
                if (typeObject == null)
                    return new OperationResult { Result = false, Error = "There is no type_tip with provided id." };

                _context.Types_TipsDb.Remove(typeObject);

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
