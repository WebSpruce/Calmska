using Calmska.Application.Abstractions;
using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Accounts.Commands;

public class CreateCommandHandler : IRequestHandler<CreateCommand, OperationResult>
{
    private readonly IRepository<Account, AccountFilter> _repository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IFirebaseService _authService;

    public CreateCommandHandler(
        IRepository<Account, AccountFilter> repository,
        IPasswordHasher passwordHasher,
        IFirebaseService authService)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _authService = authService;
    }
    
    public async Task<OperationResult> Handle(CreateCommand request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        if (string.IsNullOrEmpty(request.Password))
            return new OperationResult { Result = false, Error = "Password cannot be empty." };

        var existing = await _repository.GetByArgumentAsync(
            new AccountFilter(null, null, request.Email, null), 
            token);
        if (existing != null)
            return new OperationResult { Result = false, Error = "The user with provided email exists." };

        var hashedPassword = _passwordHasher.SetHash(request.Password);
        if(hashedPassword.Error is not null)
            return new OperationResult { Result = false, Error = hashedPassword.Error };
        
        var account = new Account
        {
            UserId = Guid.NewGuid(),
            UserName = request.UserName,
            Email = request.Email,
            PasswordHashed = hashedPassword.Hash
        };

        var persistResult = await _repository.AddAsync(account, token);
        if (!persistResult.Result)
            return persistResult;

        await _authService.CreateUserAsync(request.Email, hashedPassword.Hash);

        return new OperationResult { Result = true, Error = string.Empty };
    }
}