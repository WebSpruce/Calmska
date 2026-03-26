using Calmska.Application.Abstractions;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Accounts.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, bool>
{
    private readonly IRepository<Account, AccountFilter> _repository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IFirebaseService _authService;

    public LoginCommandHandler(IRepository<Account, AccountFilter> repository, IPasswordHasher passwordHasher, IFirebaseService authService)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _authService = authService;
    }

    public async Task<bool> Handle(LoginCommand request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var user = await _repository.GetByArgumentAsync(
            new AccountFilter(null, null, request.Email, null), token);
        if (user is null || string.IsNullOrEmpty(user.PasswordHashed))
            return false;

        var result = _passwordHasher.VerifyPassword(request.Password, user.PasswordHashed);
        if (!result.Verified)
            return false;

        var signInResult = await _authService.SignInAsync(request.Email, user.PasswordHashed);
        return signInResult.IsSuccess;
    }
}