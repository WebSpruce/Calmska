using Calmska.Domain.Common;
using MediatR;

namespace Calmska.Application.Features.Types_Tips.Commands;

public record UpdateCommand(int? TypeId, string? Type) : IRequest<OperationResult>;