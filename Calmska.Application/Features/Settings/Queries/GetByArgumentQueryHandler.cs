using Calmska.Application.DTO;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Settings.Queries;

public class GetByArgumentQueryHandler : IRequestHandler<GetByArgumentQuery, SettingsDTO?>
{
    private readonly ISettingsRepository<Domain.Entities.Settings, SettingsDTO, SettingsFilter> _repository;

    public GetByArgumentQueryHandler(ISettingsRepository<Domain.Entities.Settings, SettingsDTO, SettingsFilter> repository)
    {
        _repository = repository;
    }
    public async Task<SettingsDTO?> Handle(GetByArgumentQuery request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        return await _repository.GetByArgumentAsync(
            new SettingsFilter(request.SettingsId, request.Color, request.PomodoroTimer, request.PomodoroBreak, request.UserId), 
            token);
    }
}