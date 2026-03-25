using Calmska.Domain.Common;
using MediatR;

namespace Calmska.Application.Features.Accounts.Commands;

public record UpdateCommand(Guid? UserId, string? UserName, string? Email, string? PasswordHashed) : IRequest<OperationResult>;