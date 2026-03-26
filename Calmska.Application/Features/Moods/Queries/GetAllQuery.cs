using Calmska.Application.DTO;
using Calmska.Domain.Common;
using MediatR;

namespace Calmska.Application.Features.Moods.Queries;

public record GetAllQuery(int? PageNumber, int? PageSize) : IRequest<PaginatedResult<MoodDTO>>;