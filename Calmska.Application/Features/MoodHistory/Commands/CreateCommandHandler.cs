using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.MoodHistory.Commands;

public class CreateCommandHandler : IRequestHandler<CreateCommand, OperationResult>
{
    private readonly IRepository<Domain.Entities.MoodHistory, MoodHistoryFilter> _repository;

    public CreateCommandHandler(IRepository<Domain.Entities.MoodHistory, MoodHistoryFilter> repository)
    {
        _repository = repository;
    }
    
    public async Task<OperationResult> Handle(CreateCommand request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        if (request.MoodId == Guid.Empty || request.UserId == Guid.Empty)
            return new OperationResult { Result = false, Error = "The provided MoodHistory has empty parameters" };

        return await _repository.AddAsync(
            new Domain.Entities.MoodHistory{ MoodHistoryId = Guid.NewGuid(), Date = DateTime.UtcNow, MoodId = request.MoodId, UserId = request.UserId },
            token);
    }
}