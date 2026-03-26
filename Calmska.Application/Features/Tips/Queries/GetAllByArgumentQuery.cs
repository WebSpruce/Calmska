using Calmska.Application.DTO;
using Calmska.Domain.Common;
using MediatR;

namespace Calmska.Application.Features.Tips.Queries;

public record GetAllByArgumentQuery(Guid? TipId, string? Content, int? TipsTypeId, int? PageNumber, int? PageSize) : IRequest<PaginatedResult<TipsDTO>>;