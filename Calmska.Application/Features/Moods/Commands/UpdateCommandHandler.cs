using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Moods.Commands;

public class UpdateCommandHandler : IRequestHandler<UpdateCommand, OperationResult>
{
    private readonly IRepository<Mood, MoodFilter> _repository;

    public UpdateCommandHandler(IRepository<Mood, MoodFilter> repository)
    {
        _repository = repository;
    }
    public async Task<OperationResult> Handle(UpdateCommand request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        if (request.MoodId is null || request.MoodId == Guid.Empty)
            return new OperationResult { Result = false, Error = "The provided Mood object is empty" };

        return await _repository.UpdateAsync(
            new MoodFilter(request.MoodId, request.MoodName, request.MoodTypeId), token);
    }
}