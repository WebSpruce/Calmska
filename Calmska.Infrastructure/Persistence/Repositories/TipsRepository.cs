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
    public class TipsRepository : IRepository<Tips, TipsFilter>
    {
        private readonly CalmskaDbContext _context;
        private readonly IMapper _mapper;
        
        private const int _defaultPageNumber = 1;
        public TipsRepository(CalmskaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PaginatedResult<Tips>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await Task.Run(_context.TipsDb.AsQueryable, token);
            
            var documentResult = Pagination.Paginate(query, pageNumber, pageSize);
            var domainItems = _mapper.Map<IEnumerable<Tips>>(documentResult.Items);
            
            return new PaginatedResult<Tips>(domainItems, documentResult.TotalCount, pageNumber ?? _defaultPageNumber, pageSize ?? query.Count());
        }

        public async Task<PaginatedResult<Tips>> GetAllByArgumentAsync(TipsFilter tips, int? pageNumber, int? pageSize, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await Task.Run(_context.TipsDb
                .Where(item =>
                    (!tips.TipId.HasValue || item.TipId == tips.TipId) &&
                    (string.IsNullOrEmpty(tips.Content) || (item.Content != null && item.Content.ToLower().Contains(tips.Content.ToLower())) ) &&
                    (tips.TipsTypeId == null || tips.TipsTypeId <= 0 || item.TipsTypeId == tips.TipsTypeId)
                )
                .AsQueryable, token);

            var documentResult = Pagination.Paginate(query, pageNumber, pageSize);
            var domainItems = _mapper.Map<IEnumerable<Tips>>(documentResult.Items);
            
            return new PaginatedResult<Tips>(domainItems, documentResult.TotalCount, pageNumber ?? _defaultPageNumber, pageSize ?? query.Count());
        }

        public async Task<Tips?> GetByArgumentAsync(TipsFilter tips, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await _context.TipsDb
                .Where(item =>
                    (!tips.TipId.HasValue || item.TipId == tips.TipId) &&
                    (string.IsNullOrEmpty(tips.Content) || (item.Content != null && item.Content.ToLower().Contains(tips.Content.ToLower())) ) &&
                    (tips.TipsTypeId == null || tips.TipsTypeId <= 0 || item.TipsTypeId == tips.TipsTypeId)
                )
                .FirstOrDefaultAsync(token);
            
            return _mapper.Map<Tips?>(query);
        }

        public async Task<OperationResult> AddAsync(Tips tips, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var document = _mapper.Map<TipsDocument>(tips);
                await _context.TipsDb.AddAsync(document, token);

                await _context.SaveChangesAsync(token);
                return new OperationResult { Result = true, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> UpdateAsync(TipsFilter tips, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var existingTip = await _context.TipsDb.FirstOrDefaultAsync(t => t.TipId == tips.TipId, token);
                if (existingTip == null)
                    return new OperationResult { Result = false, Error = "Didn't find any tip with the provided tipsId." };

                existingTip.Content = tips.Content ?? string.Empty;
                existingTip.TipsTypeId = tips.TipsTypeId ?? 0;

                await _context.SaveChangesAsync(token);
                return new OperationResult { Result = true, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> DeleteAsync(Guid tipsId, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                if (tipsId == Guid.Empty)
                    return new OperationResult { Result = false, Error = "The provided TipsId is null." };

                var tipObject = await _context.TipsDb.FirstOrDefaultAsync(t => t.TipId == tipsId, token);
                if (tipObject == null)
                    return new OperationResult { Result = false, Error = "There is no tip with provided id." };

                _context.TipsDb.Remove(tipObject);

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
