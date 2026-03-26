using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Types_Mood.Queries;

public class GetByArgumentQueryHandler : IRequestHandler<GetByArgumentQuery, Domain.Entities.Types_Mood?>
{
    private readonly ITypesRepository<Domain.Entities.Types_Mood, Types_MoodFilter> _repository;

    public GetByArgumentQueryHandler(ITypesRepository<Domain.Entities.Types_Mood, Types_MoodFilter> repository)
    {
        _repository = repository;
    }
    public async Task<Domain.Entities.Types_Mood?> Handle(GetByArgumentQuery request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        return await _repository.GetByArgumentAsync(
            new Types_MoodFilter(request.TypeId, request.Type), 
            token);
    }
}