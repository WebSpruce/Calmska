using MediatR;

namespace Calmska.Application.Features.Prompts.Queries;

public record GetPromptResponseCommand(string Prompt, bool IsAnalize, bool IsMoodEntry) : IRequest<string>;