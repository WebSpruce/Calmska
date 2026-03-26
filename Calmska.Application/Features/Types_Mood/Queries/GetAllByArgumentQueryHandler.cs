using AutoMapper;
using Calmska.Application.DTO;
using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Types_Mood.Queries;

public class GetAllByArgumentQueryHandler : IRequestHandler<GetAllByArgumentQuery, PaginatedResult<Types_MoodDTO>>
{
    private readonly ITypesRepository<Domain.Entities.Types_Mood, Types_MoodFilter> _repository;
    private readonly IMapper _mapper;

    public GetAllByArgumentQueryHandler(ITypesRepository<Domain.Entities.Types_Mood, Types_MoodFilter> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<PaginatedResult<Types_MoodDTO>> Handle(GetAllByArgumentQuery request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        var result = await _repository.GetAllByArgumentAsync(
            new Types_MoodFilter(request.TypeId, request.Type),
            request.PageNumber, request.PageSize, token);
        var dtoItems = _mapper.Map<IEnumerable<Types_MoodDTO>>(result.Items);
        
        return new PaginatedResult<Types_MoodDTO>(dtoItems, result.TotalCount, result.PageNumber, result.PageSize);
    }
}