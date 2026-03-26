using Calmska.Application.DTO;
using Calmska.Domain.Common;
using MediatR;

namespace Calmska.Application.Features.Types_Tips.Queries;

public record GetAllQuery(int? PageNumber, int? PageSize) : IRequest<PaginatedResult<Types_TipsDTO>>;