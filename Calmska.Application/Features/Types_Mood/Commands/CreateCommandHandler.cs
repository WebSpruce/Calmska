using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Types_Mood.Commands;

public class CreateCommandHandler : IRequestHandler<CreateCommand, OperationResult>
{
    private readonly ITypesRepository<Domain.Entities.Types_Mood, Types_MoodFilter> _repository;

    public CreateCommandHandler(ITypesRepository<Domain.Entities.Types_Mood, Types_MoodFilter> repository)
    {
        _repository = repository;
    }
    public async Task<OperationResult> Handle(CreateCommand request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        if (string.IsNullOrEmpty(request.Type))
            return new OperationResult { Result = false, Error = "The provided object has empty parameters" };

        return await _repository.AddAsync(
            new Domain.Entities.Types_Mood { TypeId = request.TypeId, Type = request.Type},
            token);
    }
}