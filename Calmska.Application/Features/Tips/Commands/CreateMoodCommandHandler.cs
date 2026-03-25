using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Tips.Commands;

public class CreateMoodCommandHandler : IRequestHandler<CreateMoodCommand, OperationResult>
{
    private readonly IRepository<Domain.Entities.Tips, TipsFilter> _repository;

    public CreateMoodCommandHandler(IRepository<Domain.Entities.Tips, TipsFilter> repository)
    {
        _repository = repository;
    }
    public async Task<OperationResult> Handle(CreateMoodCommand request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        if (string.IsNullOrEmpty(request.Content) || request.TipsTypeId <= 0)
            return new OperationResult { Result = false, Error = "The provided Mood has empty parameters" };

        return await _repository.AddAsync(
            new Domain.Entities.Tips { TipId = request.TipId, Content = request.Content, TipsTypeId = request.TipsTypeId },
            token);
    }
}