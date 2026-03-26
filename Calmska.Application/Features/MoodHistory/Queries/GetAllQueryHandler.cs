using AutoMapper;
using Calmska.Application.DTO;
using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.MoodHistory.Queries;

public class GetAllQueryHandler : IRequestHandler<GetAllQuery, PaginatedResult<MoodHistoryDTO>>
{
    private readonly IRepository<Domain.Entities.MoodHistory, MoodHistoryFilter> _repository;
    private readonly IMapper _mapper;

    public GetAllQueryHandler(IRepository<Domain.Entities.MoodHistory, MoodHistoryFilter> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<MoodHistoryDTO>> Handle(GetAllQuery request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        var result = await _repository.GetAllAsync(request.PageNumber, request.PageSize, token);
        var dtoItems = _mapper.Map<IEnumerable<MoodHistoryDTO>>(result.Items);
        
        return new PaginatedResult<MoodHistoryDTO>(dtoItems, result.TotalCount, result.PageNumber, result.PageSize);
    }
}