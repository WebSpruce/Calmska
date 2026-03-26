using Calmska.Application.DTO;
using Calmska.Domain.Common;
using MediatR;

namespace Calmska.Application.Features.Accounts.Queries;

public record GetAllByArgumentQuery(Guid? UserId, string? UserName, string? Email, string? PasswordHashed, int? PageNumber, int? PageSize) : IRequest<PaginatedResult<AccountDTO>>;