using AutoMapper;
using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using Calmska.Infrastructure.Persistence;
using Calmska.Infrastructure.Persistence.Models;
using Calmska.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace Calmska.Infrastructure.Persistence.Repositories
{
    public class Types_TipsRepository : ITypesRepository<Types_Tips, Types_TipsFilter>
    {
        private readonly CalmskaDbContext _context;
        private readonly IMapper _mapper;
        
        private const int _defaultPageNumber = 1;
        public Types_TipsRepository(CalmskaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PaginatedResult<Types_Tips>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await Task.Run(_context.Types_TipsDb.AsQueryable, token);
            
            var documentResult = Pagination.Paginate(query, pageNumber, pageSize);
            var domainItems = _mapper.Map<IEnumerable<Types_Tips>>(documentResult.Items);
            
            return new PaginatedResult<Types_Tips>(domainItems, documentResult.TotalCount, pageNumber ?? _defaultPageNumber, pageSize ?? query.Count());
        }

        public async Task<PaginatedResult<Types_Tips>> GetAllByArgumentAsync(Types_TipsFilter tips, int? pageNumber, int? pageSize,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await Task.Run(_context.Types_TipsDb
                .Where(item =>
                    (!tips.TypeId.HasValue || item.TypeId == tips.TypeId) &&
                    (string.IsNullOrEmpty(tips.Type) || item.Type.ToLower().Contains(tips.Type.ToLower()))
                )
                .AsQueryable, token);

            var documentResult = Pagination.Paginate(query, pageNumber, pageSize);
            var domainItems = _mapper.Map<IEnumerable<Types_Tips>>(documentResult.Items);
            
            return new PaginatedResult<Types_Tips>(domainItems, documentResult.TotalCount, pageNumber ?? _defaultPageNumber, pageSize ?? query.Count());
        }

        public async Task<Types_Tips?> GetByArgumentAsync(Types_TipsFilter tips, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await _context.Types_TipsDb
                .Where(item =>
                    (!tips.TypeId.HasValue || item.TypeId == tips.TypeId) &&
                    (string.IsNullOrEmpty(tips.Type) || item.Type.ToLower().Contains(tips.Type.ToLower()))
                )
                .FirstOrDefaultAsync(token);
            
            return _mapper.Map<Types_Tips?>(query);
        }

        public async Task<OperationResult> AddAsync(Types_Tips TypesTips, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                int maxTypeId = 0;
                if (await _context.Types_TipsDb.AnyAsync(token))
                {
                    var items = await _context.Types_TipsDb.ToListAsync(token);
                    if (items.Any())
                    {
                        maxTypeId = items.MaxBy(t => t.TypeId).TypeId;
                    }
                }

                var document = _mapper.Map<Types_TipsDocument>(TypesTips);
                document.TypeId = maxTypeId + 1;
                await _context.Types_TipsDb.AddAsync(document, token);

                await _context.SaveChangesAsync(token);
                return new OperationResult { Result = true, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> UpdateAsync(Types_TipsFilter tips, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                if (tips == null)
                    return new OperationResult { Result = false, Error = "The provided Type_Tip object is null." };

                var existingTypeTip = await _context.Types_TipsDb.FirstOrDefaultAsync(t => t.TypeId == tips.TypeId, token);
                if (existingTypeTip == null)
                    return new OperationResult { Result = false, Error = "Didn't find any type_tip with the provided type_tipsId." };

                existingTypeTip.Type = tips.Type ?? string.Empty;

                await _context.SaveChangesAsync(token);
                return new OperationResult { Result = true, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> DeleteAsync(int id, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var typeObject = await _context.Types_TipsDb.FirstOrDefaultAsync(t => t.TypeId == id, token);
                if (typeObject == null)
                    return new OperationResult { Result = false, Error = "There is no type_tip with provided id." };

                _context.Types_TipsDb.Remove(typeObject);

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
