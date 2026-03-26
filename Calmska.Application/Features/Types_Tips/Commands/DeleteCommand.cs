using Calmska.Domain.Common;
using MediatR;

namespace Calmska.Application.Features.Types_Tips.Commands;

public record DeleteCommand(int Id) : IRequest<OperationResult>;