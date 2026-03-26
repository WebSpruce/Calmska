using Calmska.Application.Abstractions;
using MediatR;

namespace Calmska.Application.Features.Prompts.Queries;

public class GetPromptResponseCommandHandler : IRequestHandler<GetPromptResponseCommand, string>
{
    private readonly IAiPromptingRepository _repository;

    public GetPromptResponseCommandHandler(IAiPromptingRepository repository)
    {
        _repository = repository;
    }

    public async Task<string> Handle(GetPromptResponseCommand request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        return await _repository.GetPromptResponseAsync(request.Prompt, request.IsAnalize, request.IsMoodEntry, token);
    }
}