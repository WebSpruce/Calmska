using Calmska.Domain.Common;
using MediatR;

namespace Calmska.Application.Features.MoodHistory.Commands;

public record DeleteCommand(Guid Id) : IRequest<OperationResult>;