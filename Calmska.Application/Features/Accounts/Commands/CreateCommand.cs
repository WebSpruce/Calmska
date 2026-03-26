using Calmska.Domain.Common;
using MediatR;

namespace Calmska.Application.Features.Accounts.Commands;

public sealed record CreateCommand(string UserName, string Email, string Password) : IRequest<OperationResult>;