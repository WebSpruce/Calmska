using Calmska.Application.DTO;
using Calmska.Domain.Common;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Settings.Commands;

public class UpdateCommandHandler : IRequestHandler<UpdateCommand, OperationResult>
{
    private readonly ISettingsRepository<Domain.Entities.Settings, SettingsDTO, SettingsFilter> _repository;

    public UpdateCommandHandler(ISettingsRepository<Domain.Entities.Settings, SettingsDTO, SettingsFilter> repository)
    {
        _repository = repository;
    }
    public async Task<OperationResult> Handle(UpdateCommand request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        if (request.SettingsId is null || request.SettingsId == Guid.Empty)
            return new OperationResult { Result = false, Error = "The provided Id object is empty" };

        return await _repository.UpdateAsync(
            new SettingsFilter(request.SettingsId, request.Color, request.PomodoroTimer, request.PomodoroBreak, request.UserId), token); 
    }
}