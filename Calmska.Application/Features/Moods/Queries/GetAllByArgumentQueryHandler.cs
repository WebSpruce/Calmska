using AutoMapper;
using Calmska.Application.DTO;
using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Moods.Queries;

public class GetAllByArgumentQueryHandler : IRequestHandler<GetAllByArgumentQuery, PaginatedResult<MoodDTO>>
{
    private readonly IRepository<Mood, MoodFilter> _repository;
    private readonly IMapper _mapper;

    public GetAllByArgumentQueryHandler(IRepository<Mood, MoodFilter> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<PaginatedResult<MoodDTO>> Handle(GetAllByArgumentQuery request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        var result = await _repository.GetAllByArgumentAsync(
            new MoodFilter(request.MoodId, request.MoodName, request.MoodTypeId),
            request.PageNumber, request.PageSize, token);
        var dtoItems = _mapper.Map<IEnumerable<MoodDTO>>(result.Items);
        
        return new PaginatedResult<MoodDTO>(dtoItems, result.TotalCount, result.PageNumber, result.PageSize);
    }
}