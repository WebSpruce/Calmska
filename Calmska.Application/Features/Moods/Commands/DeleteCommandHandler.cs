using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Moods.Commands;

public class DeleteCommandHandler : IRequestHandler<DeleteCommand, OperationResult>
{
    private readonly IRepository<Mood, MoodFilter> _repository;

    public DeleteCommandHandler(IRepository<Mood, MoodFilter> repository)
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