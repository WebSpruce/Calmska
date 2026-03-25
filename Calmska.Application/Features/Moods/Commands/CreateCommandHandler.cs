using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Moods.Commands;

public class CreateCommandHandler : IRequestHandler<CreateCommand, OperationResult>
{
    private readonly IRepository<Mood, MoodFilter> _repository;

    public CreateCommandHandler(IRepository<Mood, MoodFilter> repository)
    {
        _repository = repository;
    }
    public async Task<OperationResult> Handle(CreateCommand request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        if (string.IsNullOrEmpty(request.MoodName) || request.MoodTypeId <= 0)
            return new OperationResult { Result = false, Error = "The provided Mood has empty parameters" };

        return await _repository.AddAsync(
            new Mood { MoodId = Guid.NewGuid(), MoodName = request.MoodName, MoodTypeId = request.MoodTypeId },
            token);
    }
}