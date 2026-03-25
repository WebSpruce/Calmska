using AutoMapper;
using Calmska.Application.DTO;
using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Types_Tips.Queries;

public class GetAllByArgumentQueryHandler : IRequestHandler<GetAllByArgumentQuery, PaginatedResult<Types_TipsDTO>>
{
    private readonly ITypesRepository<Domain.Entities.Types_Tips, Types_TipsFilter> _repository;
    private readonly IMapper _mapper;

    public GetAllByArgumentQueryHandler(ITypesRepository<Domain.Entities.Types_Tips, Types_TipsFilter> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<PaginatedResult<Types_TipsDTO>> Handle(GetAllByArgumentQuery request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        var result = await _repository.GetAllByArgumentAsync(
            new Types_TipsFilter(request.TypeId, request.Type),
            request.PageNumber, request.PageSize, token);
        var dtoItems = _mapper.Map<IEnumerable<Types_TipsDTO>>(result.Items);
        
        return new PaginatedResult<Types_TipsDTO>(dtoItems, result.TotalCount, result.PageNumber, result.PageSize);
    }
}