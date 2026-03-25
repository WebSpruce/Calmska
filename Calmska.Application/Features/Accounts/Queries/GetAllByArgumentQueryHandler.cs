using AutoMapper;
using Calmska.Application.DTO;
using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Accounts.Queries;

public class GetAllByArgumentQueryHandler : IRequestHandler<GetAllByArgumentQuery, PaginatedResult<AccountDTO>>
{
    private readonly IRepository<Account, AccountFilter> _repository;
    private readonly IMapper _mapper;

    public GetAllByArgumentQueryHandler(IRepository<Account, AccountFilter> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<AccountDTO>> Handle(GetAllByArgumentQuery request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var result = await _repository.GetAllByArgumentAsync(
            new AccountFilter(request.UserId, request.UserName, request.Email, request.PasswordHashed),
            request.PageNumber, request.PageSize, 
            token);
        var dtoItems = _mapper.Map<IEnumerable<AccountDTO>>(result.Items);
        
        return new PaginatedResult<AccountDTO>(dtoItems, result.TotalCount, result.PageNumber, result.PageSize);
    }
}