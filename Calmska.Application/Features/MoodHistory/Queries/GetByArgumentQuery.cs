using Calmska.Domain.Entities;
using MediatR;

namespace Calmska.Application.Features.MoodHistory.Queries;

public record GetByArgumentQuery(Guid? MoodHistoryId, DateTime? Date, Guid? UserId, Guid? MoodId) : IRequest<Domain.Entities.MoodHistory?>;