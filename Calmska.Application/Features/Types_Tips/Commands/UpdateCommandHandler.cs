using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Types_Tips.Commands;

public class UpdateCommandHandler : IRequestHandler<UpdateCommand, OperationResult>
{
    private readonly ITypesRepository<Domain.Entities.Types_Tips, Types_TipsFilter> _repository;

    public UpdateCommandHandler(ITypesRepository<Domain.Entities.Types_Tips, Types_TipsFilter> repository)
    {
        _repository = repository;
    }
    public async Task<OperationResult> Handle(UpdateCommand request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        if (request.TypeId is null || request.TypeId <= 0)
            return new OperationResult { Result = false, Error = "The provided Mood object is empty" };

        return await _repository.UpdateAsync(
            new Types_TipsFilter(request.TypeId, request.Type), token);
    }
}