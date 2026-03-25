using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Moods.Queries;

public class GetByArgumentQueryHandler : IRequestHandler<GetByArgumentQuery, Mood?>
{
    private readonly IRepository<Mood, MoodFilter> _repository;

    public GetByArgumentQueryHandler(IRepository<Mood, MoodFilter> repository)
    {
        _repository = repository;
    }
    public async Task<Mood?> Handle(GetByArgumentQuery request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        return await _repository.GetByArgumentAsync(
            new MoodFilter(request.MoodId, request.MoodName, request.MoodTypeId), 
            token);
    }
}