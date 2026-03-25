using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Tips.Queries;

public class GetByArgumentQueryHandler : IRequestHandler<GetByArgumentQuery, Domain.Entities.Tips?>
{
    private readonly IRepository<Domain.Entities.Tips, TipsFilter> _repository;

    public GetByArgumentQueryHandler(IRepository<Domain.Entities.Tips, TipsFilter> repository)
    {
        _repository = repository;
    }
    public async Task<Domain.Entities.Tips?> Handle(GetByArgumentQuery request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        return await _repository.GetByArgumentAsync(
            new TipsFilter(request.TipId, request.Content, request.TipsTypeId), 
            token);
    }
}