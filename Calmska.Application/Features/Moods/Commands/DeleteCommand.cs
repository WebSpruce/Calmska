using Calmska.Domain.Common;
using MediatR;

namespace Calmska.Application.Features.Moods.Commands;

public record DeleteCommand(Guid Id) : IRequest<OperationResult>;