using Calmska.Domain.Common;
using MediatR;

namespace Calmska.Application.Features.Accounts.Commands;

public record DeleteCommand(Guid Id) : IRequest<OperationResult>;