using Calmska.Application.DTO;
using Calmska.Domain.Common;
using MediatR;

namespace Calmska.Application.Features.MoodHistory.Queries;

public record GetAllByArgumentQuery(Guid? MoodHistoryId, DateTime? Date, Guid? UserId, Guid? MoodId, int? PageNumber, int? PageSize) : IRequest<PaginatedResult<MoodHistoryDTO>>;