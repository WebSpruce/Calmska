using Calmska.Domain.Common;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.MoodHistory.Commands;

public class UpdateCommandHandler : IRequestHandler<UpdateCommand, OperationResult>
{
    private readonly IRepository<Domain.Entities.MoodHistory, MoodHistoryFilter> _repository;

    public UpdateCommandHandler(IRepository<Domain.Entities.MoodHistory, MoodHistoryFilter> repository)
    {
        _repository = repository;
    }

    public async Task<OperationResult> Handle(UpdateCommand request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        if (request.MoodHistoryId is null || request.MoodHistoryId == Guid.Empty)
            return new OperationResult { Result = false, Error = "MoodHistory Id cannot be empty" };
        
        return await _repository.UpdateAsync(
            new MoodHistoryFilter(request.MoodHistoryId, request.Date, request.UserId, request.MoodId),
            token);
    }
}