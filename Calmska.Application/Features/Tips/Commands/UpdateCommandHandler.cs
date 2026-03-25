using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Tips.Commands;

public class UpdateCommandHandler : IRequestHandler<UpdateCommand, OperationResult>
{
    private readonly IRepository<Domain.Entities.Tips, TipsFilter> _repository;

    public UpdateCommandHandler(IRepository<Domain.Entities.Tips, TipsFilter> repository)
    {
        _repository = repository;
    }
    public async Task<OperationResult> Handle(UpdateCommand request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        if (request.TipId is null || request.TipId == Guid.Empty)
            return new OperationResult { Result = false, Error = "The provided Tip object is null" };

        return await _repository.UpdateAsync(
            new TipsFilter(request.TipId, request.Content, request.TipsTypeId), token);
    }
}