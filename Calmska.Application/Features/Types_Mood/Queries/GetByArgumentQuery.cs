using Calmska.Domain.Entities;
using MediatR;

namespace Calmska.Application.Features.Types_Mood.Queries;

public record GetByArgumentQuery(int? TypeId, string? Type) : IRequest<Domain.Entities.Types_Mood?>;