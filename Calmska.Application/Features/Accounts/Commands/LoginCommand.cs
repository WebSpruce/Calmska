using MediatR;

namespace Calmska.Application.Features.Accounts.Commands;

public sealed record LoginCommand(string Email, string Password) : IRequest<bool>;