using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Accounts.Commands;

public class DeleteCommandHandler : IRequestHandler<DeleteCommand, OperationResult>
{ 
    private readonly IRepository<Account, AccountFilter> _repository;

    public DeleteCommandHandler(IRepository<Account, AccountFilter> repository)
    {
        _repository = repository;
    }
    public async Task<OperationResult> Handle(DeleteCommand request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        if (request.Id == Guid.Empty)
            return new OperationResult { Result = false, Error = "User id cannot be empty" };
        
        return await _repository.DeleteAsync(
            request.Id,
            token);
    }
}