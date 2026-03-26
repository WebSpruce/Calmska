using Calmska.Domain.Entities;
using MediatR;

namespace Calmska.Application.Features.Tips.Queries;

public record GetByArgumentQuery(Guid? TipId, string? Content, int? TipsTypeId) : IRequest<Domain.Entities.Tips?>;