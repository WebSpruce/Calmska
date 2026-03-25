using Calmska.Domain.Common;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Types_Tips.Commands;

public class CreateMoodCommandHandler : IRequestHandler<CreateMoodCommand, OperationResult>
{
    private readonly ITypesRepository<Domain.Entities.Types_Tips, Types_TipsFilter> _repository;

    public CreateMoodCommandHandler(ITypesRepository<Domain.Entities.Types_Tips, Types_TipsFilter> repository)
    {
        _repository = repository;
    }
    public async Task<OperationResult> Handle(CreateMoodCommand request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        if (string.IsNullOrEmpty(request.Type) || request.TypeId <= 0)
            return new OperationResult { Result = false, Error = "The provided object has empty parameters" };

        return await _repository.AddAsync(
            new Domain.Entities.Types_Tips { TypeId = request.TypeId, Type = request.Type },
            token);
    }
}