using Calmska.Domain.Entities;
using MediatR;

namespace Calmska.Application.Features.Accounts.Queries;

public record GetByArgumentQuery(Guid? UserId, string? UserName, string? Email, string? PasswordHashed) : IRequest<Account?>;