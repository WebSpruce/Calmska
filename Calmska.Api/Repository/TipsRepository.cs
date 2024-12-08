using AutoMapper;
using Calmska.Api.DTO;
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
        public async Task<PaginatedResult<Tips>> GetAllAsync(int? pageNumber, int? pageSize)
        {
            var query = await Task.Run(_context.TipsDb.AsQueryable);
            return Pagination.Paginate(query, pageNumber, pageSize);
        }

        public async Task<PaginatedResult<Tips>> GetAllByArgumentAsync(TipsDTO tips, int? pageNumber, int? pageSize)
        {
            var query = await Task.Run(_context.TipsDb
                .Where(item =>
                    (!tips.TipId.HasValue || item.TipId == tips.TipId) &&
                    (string.IsNullOrEmpty(tips.Content) || item.Content.ToLower().Contains(tips.Content.ToLower())) &&
                    (string.IsNullOrEmpty(tips.Type) || item.Type.ToLower().Contains(tips.Type.ToLower()))
                )
                .AsQueryable);

            return Pagination.Paginate(query, pageNumber, pageSize);
        }

        public async Task<Tips?> GetByArgumentAsync(TipsDTO tips)
        {
            return await _context.TipsDb
                .Where(item =>
                    (!tips.TipId.HasValue || item.TipId == tips.TipId) &&
                    (string.IsNullOrEmpty(tips.Content) || item.Content.ToLower().Contains(tips.Content.ToLower())) &&
                    (string.IsNullOrEmpty(tips.Type) || item.Type.ToLower().Contains(tips.Type.ToLower()))
                )
                .FirstOrDefaultAsync();
        }

        public async Task<OperationResult> AddAsync(TipsDTO tipsDto)
        {
            try
            {
                if (tipsDto == null)
                    return new OperationResult { Result = false, Error = "The provided Tips object is null." };

                var tips = _mapper.Map<Tips>(tipsDto);
                await _context.TipsDb.AddAsync(tips);

                var result = await _context.SaveChangesAsync();
                return new OperationResult { Result = result > 0 ? true : false, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> UpdateAsync(TipsDTO tips)
        {
            try
            {
                if (tips == null)
                    return new OperationResult { Result = false, Error = "The provided Account object is null." };

                Tips? existingTip = await _context.TipsDb.FirstOrDefaultAsync(t => t.TipId == tips.TipId);
                if (existingTip == null)
                    return new OperationResult { Result = false, Error = "Didn't find any tip with the provided tipsId." };

                existingTip.Content = tips.Content ?? string.Empty;
                existingTip.Type = tips.Type ?? string.Empty;

                var result = await _context.SaveChangesAsync();
                return new OperationResult { Result = result > 0 ? true : false, Error = string.Empty };
            }
            catch (Exception ex)
            {
                return new OperationResult { Result = false, Error = $"{ex.Message} - {ex.InnerException}" };
            }
        }

        public async Task<OperationResult> DeleteAsync(Guid tipsId)
        {
            try
            {
                if (tipsId == Guid.Empty)
                    return new OperationResult { Result = false, Error = "The provided TipsId is null." };

                var tipObject = await _context.TipsDb.FirstOrDefaultAsync(t => t.TipId == tipsId);
                if (tipObject == null)
                    return new OperationResult { Result = false, Error = "There is no tip with provided id." };

                _context.TipsDb.Remove(tipObject);

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
