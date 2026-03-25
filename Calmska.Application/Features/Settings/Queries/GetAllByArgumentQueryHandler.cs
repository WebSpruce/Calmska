using AutoMapper;
using Calmska.Application.DTO;
using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Settings.Queries;

public class GetAllByArgumentQueryHandler : IRequestHandler<GetAllByArgumentQuery, PaginatedResult<SettingsDTO>>
{
    private readonly IRepository<Domain.Entities.Settings, SettingsFilter> _repository;
    private readonly IMapper _mapper;

    public GetAllByArgumentQueryHandler(IRepository<Domain.Entities.Settings, SettingsFilter> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<PaginatedResult<SettingsDTO>> Handle(GetAllByArgumentQuery request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        var result = await _repository.GetAllByArgumentAsync(
            new SettingsFilter(request.SettingsId, request.Color, request.PomodoroTimer, request.PomodoroBreak, request.UserId),
            request.PageNumber, request.PageSize, token);
        var dtoItems = _mapper.Map<IEnumerable<SettingsDTO>>(result.Items);
        
        return new PaginatedResult<SettingsDTO>(dtoItems, result.TotalCount, result.PageNumber, result.PageSize);
    }
}