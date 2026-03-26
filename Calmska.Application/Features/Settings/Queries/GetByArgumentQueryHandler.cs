using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Settings.Queries;

public class GetByArgumentQueryHandler : IRequestHandler<GetByArgumentQuery, Domain.Entities.Settings?>
{
    private readonly IRepository<Domain.Entities.Settings, SettingsFilter> _repository;

    public GetByArgumentQueryHandler(IRepository<Domain.Entities.Settings, SettingsFilter> repository)
    {
        _repository = repository;
    }
    public async Task<Domain.Entities.Settings?> Handle(GetByArgumentQuery request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        return await _repository.GetByArgumentAsync(
            new SettingsFilter(request.SettingsId, request.Color, request.PomodoroTimer, request.PomodoroBreak, request.UserId), 
            token);
    }
}