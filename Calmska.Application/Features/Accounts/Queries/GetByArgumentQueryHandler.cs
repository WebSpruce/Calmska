using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Accounts.Queries;

public class GetByArgumentQueryHandler : IRequestHandler<GetByArgumentQuery, Account?>
{
    private readonly IRepository<Account, AccountFilter> _repository;

    public GetByArgumentQueryHandler(IRepository<Account, AccountFilter> repository)
    {
        _repository = repository;
    }
    public async Task<Account?> Handle(GetByArgumentQuery request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        return await _repository.GetByArgumentAsync(
            new AccountFilter(request.UserId, request.UserName, request.Email, request.PasswordHashed),
            token);
    }
}