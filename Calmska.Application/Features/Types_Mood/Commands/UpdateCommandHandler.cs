using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Types_Mood.Commands;

public class UpdateCommandHandler : IRequestHandler<UpdateCommand, OperationResult>
{
    private readonly ITypesRepository<Domain.Entities.Types_Mood, Types_MoodFilter> _repository;

    public UpdateCommandHandler(ITypesRepository<Domain.Entities.Types_Mood, Types_MoodFilter> repository)
    {
        _repository = repository;
    }
    public async Task<OperationResult> Handle(UpdateCommand request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        if (request.TypeId is null || request.TypeId <= 0 || string.IsNullOrEmpty(request.Type))
            return new OperationResult { Result = false, Error = "The provided object is empty" };

        return await _repository.UpdateAsync(
            new Types_MoodFilter(request.TypeId, request.Type), token);
    }
}