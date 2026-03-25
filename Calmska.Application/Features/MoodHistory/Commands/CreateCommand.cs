using Calmska.Domain.Common;
using MediatR;

namespace Calmska.Application.Features.MoodHistory.Commands;

public sealed record CreateCommand(Guid UserId, Guid MoodId) : IRequest<OperationResult>;