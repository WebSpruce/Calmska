using Calmska.Domain.Entities;
using MediatR;

namespace Calmska.Application.Features.Types_Tips.Queries;

public record GetByArgumentQuery(int? TypeId, string? Type) : IRequest<Domain.Entities.Types_Tips?>;