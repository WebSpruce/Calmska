using AutoMapper;
using Calmska.Models.DTO;
using Calmska.Api.Helper;
using Calmska.Api.Interfaces;
using Calmska.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Calmska.Api.Repository
{
    public class TipsRepository : IRepository<Tips, TipsDTO>
    {
        private readonly CalmskaDbContext _context;
        private readonly IMapper _mapper;
        public TipsRepository(CalmskaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PaginatedResult<Tips>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await Task.Run(_context.TipsDb.AsQueryable, token);
            return Pagination.Paginate(query, pageNumber, pageSize);
        }

        public async Task<PaginatedResult<Tips>> GetAllByArgumentAsync(TipsDTO tips, int? pageNumber, int? pageSize, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var query = await Task.Run(_context.TipsDb
                .Where(item =>
                    (!tips.TipId.HasValue || item.TipId == tips.TipId) &&
                    (string.IsNullOrEmpty(tips.Content) || item.Content.ToLower().Contains(tips.Content.ToLower())) &&
                    (tips.TipsTypeId == null || tips.TipsTypeId <= 0 || item.TipsTypeId == tips.TipsTypeId)
                )
                .AsQueryable, token);

            return Pagination.Paginate(query, pageNumber, pageSize);
        }

        public async Task<Tips?> GetByArgumentAsync(TipsDTO tips, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return await _context.TipsDb
                .Where(item =>
                    (!tips.TipId.HasValue || item.TipId == tips.TipId) &&
                    (string.IsNullOrEmpty(tips.Content) || item.Content.ToLower().Contains(tips.Content.ToLower())) &&
                    (tips.TipsTypeId == null || tips.TipsTypeId <= 0 || item.TipsTypeId == tips.TipsTypeId)
                )
                .FirstOrDefaultAsync(token);
        }

        public async Task<OperationResult> AddAsync(TipsDTO tipsDto, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                if (tipsDto == null)
                    return new OperationResult { Result = false, Error = "The provided Tips object is null." };

                var tips = _mapper.Map<Tips>(tipsDto);
                await _context.TipsDb.AddAsync(tips, token);

                var result = await _context.SaveChangesAsync(token);
                return new OperationResult { Result = result > 0 ? true : false, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> UpdateAsync(TipsDTO tips, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                if (tips == null)
                    return new OperationResult { Result = false, Error = "The provided Tip object is null." };

                Tips? existingTip = await _context.TipsDb.FirstOrDefaultAsync(t => t.TipId == tips.TipId, token);
                if (existingTip == null)
                    return new OperationResult { Result = false, Error = "Didn't find any tip with the provided tipsId." };

                existingTip.Content = tips.Content ?? string.Empty;
                existingTip.TipsTypeId = tips.TipsTypeId ?? 0;

                var result = await _context.SaveChangesAsync(token);
                return new OperationResult { Result = result > 0 ? true : false, Error = string.Empty };
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
