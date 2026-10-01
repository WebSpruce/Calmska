using Calmska.Application.DTO;
using Calmska.Domain.Common;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Settings.Commands;

public class DeleteCommandHandler : IRequestHandler<DeleteCommand, OperationResult>
{
    private readonly ISettingsRepository<Domain.Entities.Settings, SettingsDTO, SettingsFilter> _repository;

    public DeleteCommandHandler(ISettingsRepository<Domain.Entities.Settings, SettingsDTO, SettingsFilter> repository)
    {
        _repository = repository;
    }
    public async Task<OperationResult> Handle(DeleteCommand request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        if (request.Id == Guid.Empty)
            return new OperationResult { Result = false, Error = "The provided Id empty" };

        return await _repository.DeleteAsync(request.Id, token);
    }
}