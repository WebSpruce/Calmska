using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Settings.Commands;

public class CreateMoodCommandHandler : IRequestHandler<CreateMoodCommand, OperationResult>
{
    private readonly IRepository<Domain.Entities.Settings, SettingsFilter> _repository;

    public CreateMoodCommandHandler(IRepository<Domain.Entities.Settings, SettingsFilter> repository)
    {
        _repository = repository;
    }
    public async Task<OperationResult> Handle(CreateMoodCommand request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        if (request.UserId == Guid.Empty)
            return new OperationResult { Result = false, Error = "The provided userId parameter is empty" };
        
        var settingsByUserId = _repository.GetByArgumentAsync(
            new SettingsFilter(null, null, null, null, request.UserId), 
            token);
        if (settingsByUserId.Result != null)
            return new OperationResult { Result = false, Error = $"The settings object exists for the user with id: {request.UserId}." };

        return await _repository.AddAsync(
            new Domain.Entities.Settings { Color = request.Color, PomodoroBreak = request.PomodoroBreak, PomodoroTimer = request.PomodoroTimer, SettingsId = request.SettingsId, UserId = request.UserId},
            token);
    }
}