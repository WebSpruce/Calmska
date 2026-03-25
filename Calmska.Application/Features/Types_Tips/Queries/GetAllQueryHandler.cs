using AutoMapper;
using Calmska.Application.DTO;
using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Types_Tips.Queries;

public class GetAllQueryHandler : IRequestHandler<GetAllQuery, PaginatedResult<Types_TipsDTO>>
{
    private readonly ITypesRepository<Domain.Entities.Types_Tips, Types_TipsFilter> _repository;
    private readonly IMapper _mapper;

    public GetAllQueryHandler(ITypesRepository<Domain.Entities.Types_Tips, Types_TipsFilter> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<Types_TipsDTO>> Handle(GetAllQuery request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        var result = await _repository.GetAllAsync(request.PageNumber, request.PageSize, token);
        var dtoItems = _mapper.Map<IEnumerable<Types_TipsDTO>>(result.Items);
        
        return new PaginatedResult<Types_TipsDTO>(dtoItems, result.TotalCount, result.PageNumber, result.PageSize);
    }
}