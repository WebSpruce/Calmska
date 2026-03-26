using Calmska.Domain.Common;
using MediatR;

namespace Calmska.Application.Features.Tips.Commands;

public record UpdateCommand(Guid? TipId, string? Content, int? TipsTypeId) : IRequest<OperationResult>;