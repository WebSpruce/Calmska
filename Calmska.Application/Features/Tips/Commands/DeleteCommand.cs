using Calmska.Domain.Common;
using MediatR;

namespace Calmska.Application.Features.Tips.Commands;

public record DeleteCommand(Guid Id) : IRequest<OperationResult>;