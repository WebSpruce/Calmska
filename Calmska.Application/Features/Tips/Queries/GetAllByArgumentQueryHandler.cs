using AutoMapper;
using Calmska.Application.DTO;
using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Tips.Queries;

public class GetAllByArgumentQueryHandler : IRequestHandler<GetAllByArgumentQuery, PaginatedResult<TipsDTO>>
{
    private readonly IRepository<Domain.Entities.Tips, TipsFilter> _repository;
    private readonly IMapper _mapper;

    public GetAllByArgumentQueryHandler(IRepository<Domain.Entities.Tips, TipsFilter> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<PaginatedResult<TipsDTO>> Handle(GetAllByArgumentQuery request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        var result = await _repository.GetAllByArgumentAsync(
            new TipsFilter(request.TipId, request.Content, request.TipsTypeId),
            request.PageNumber, request.PageSize, token);
        var dtoItems = _mapper.Map<IEnumerable<TipsDTO>>(result.Items);
        
        return new PaginatedResult<TipsDTO>(dtoItems, result.TotalCount, result.PageNumber, result.PageSize);
    }
}