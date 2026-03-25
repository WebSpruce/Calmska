using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Types_Tips.Queries;

public class GetByArgumentQueryHandler : IRequestHandler<GetByArgumentQuery, Domain.Entities.Types_Tips?>
{
    private readonly ITypesRepository<Domain.Entities.Types_Tips, Types_TipsFilter> _repository;

    public GetByArgumentQueryHandler(ITypesRepository<Domain.Entities.Types_Tips, Types_TipsFilter> repository)
    {
        _repository = repository;
    }
    public async Task<Domain.Entities.Types_Tips?> Handle(GetByArgumentQuery request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        return await _repository.GetByArgumentAsync(
            new Types_TipsFilter(request.TypeId, request.Type), 
            token);
    }
}