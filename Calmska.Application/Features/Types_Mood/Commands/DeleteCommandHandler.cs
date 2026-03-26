using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Types_Mood.Commands;

public class DeleteCommandHandler : IRequestHandler<DeleteCommand, OperationResult>
{
    private readonly ITypesRepository<Domain.Entities.Types_Mood, Types_MoodFilter> _repository;

    public DeleteCommandHandler(ITypesRepository<Domain.Entities.Types_Mood, Types_MoodFilter> repository)
    {
        _repository = repository;
    }
    public async Task<OperationResult> Handle(DeleteCommand request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        if (request.Id <= 0)
            return new OperationResult { Result = false, Error = "The provided Id empty" };

        return await _repository.DeleteAsync(request.Id, token);
    }
}