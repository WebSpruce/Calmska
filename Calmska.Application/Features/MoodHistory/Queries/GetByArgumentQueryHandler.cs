using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.MoodHistory.Queries;

public class GetByArgumentQueryHandler : IRequestHandler<GetByArgumentQuery, Domain.Entities.MoodHistory?>
{
    private readonly IRepository<Domain.Entities.MoodHistory, MoodHistoryFilter> _repository;

    public GetByArgumentQueryHandler(IRepository<Domain.Entities.MoodHistory, MoodHistoryFilter> repository)
    {
        _repository = repository;
    }
    public async Task<Domain.Entities.MoodHistory?> Handle(GetByArgumentQuery request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        return await _repository.GetByArgumentAsync(
            new MoodHistoryFilter(request.MoodHistoryId, request.Date, request.MoodId, request.UserId), 
            token);
    }
}