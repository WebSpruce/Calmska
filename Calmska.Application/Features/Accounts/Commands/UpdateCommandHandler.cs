using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Accounts.Commands;

public class UpdateCommandHandler : IRequestHandler<UpdateCommand, OperationResult>
{
    private readonly IRepository<Account, AccountFilter> _repository;

    public UpdateCommandHandler(IRepository<Account, AccountFilter> repository)
    {
        _repository = repository;
    }

    public async Task<OperationResult> Handle(UpdateCommand request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        if (request.UserId is null || request.UserId == Guid.Empty)
            return new OperationResult { Result = false, Error = "User id cannot be empty" };
        
        return await _repository.UpdateAsync(
            new AccountFilter(request.UserId, request.UserName, request.Email, request.PasswordHashed),
            token);
    }
}