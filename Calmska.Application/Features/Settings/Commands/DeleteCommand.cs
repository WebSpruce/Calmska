using Calmska.Domain.Common;
using MediatR;

namespace Calmska.Application.Features.Settings.Commands;

public record DeleteCommand(Guid Id) : IRequest<OperationResult>;