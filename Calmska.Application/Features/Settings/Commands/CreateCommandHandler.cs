using System.Globalization;
using Calmska.Application.DTO;
using Calmska.Domain.Common;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Settings.Commands;

public class CreateCommandHandler : IRequestHandler<CreateCommand, OperationResult>
{
    private readonly ISettingsRepository<Domain.Entities.Settings, SettingsDTO, SettingsFilter> _repository;

    public CreateCommandHandler(ISettingsRepository<Domain.Entities.Settings, SettingsDTO, SettingsFilter> repository)
    {
        _repository = repository;
    }
    public async Task<OperationResult> Handle(CreateCommand request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        if (request.UserId == Guid.Empty)
            return new OperationResult { Result = false, Error = "The provided userId parameter is empty" };
        
        var settingsByUserId = await _repository.GetByArgumentAsync(
            new SettingsFilter(null, null, null, null, request.UserId), 
            token);
        if (settingsByUserId != null)
            return new OperationResult { Result = false, Error = $"The settings object exists for the user with id: {request.UserId}." };

        return await _repository.AddAsync(
            new Domain.Entities.Settings { Color = request.Color, PomodoroBreak = request.PomodoroBreak.ToString(CultureInfo.InvariantCulture), PomodoroTimer = request.PomodoroTimer.ToString(CultureInfo.InvariantCulture), SettingsId = request.SettingsId, UserId = request.UserId},
            token);
    }
}