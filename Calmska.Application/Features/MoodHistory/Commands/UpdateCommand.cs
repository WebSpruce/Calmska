using Calmska.Domain.Common;
using MediatR;

namespace Calmska.Application.Features.MoodHistory.Commands;

public record UpdateCommand(Guid? MoodHistoryId, DateTime? Date, Guid? UserId, Guid? MoodId) : IRequest<OperationResult>;