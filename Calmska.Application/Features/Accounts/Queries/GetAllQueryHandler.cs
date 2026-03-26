using AutoMapper;
using Calmska.Application.DTO;
using Calmska.Domain.Common;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using MediatR;

namespace Calmska.Application.Features.Accounts.Queries;

public class GetAllQueryHandler : IRequestHandler<GetAllQuery, PaginatedResult<AccountDTO>>
{
    private readonly IRepository<Account, AccountFilter> _repository;
    private readonly IMapper _mapper;

    public GetAllQueryHandler(IRepository<Account, AccountFilter> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<AccountDTO>> Handle(GetAllQuery request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var result = await _repository.GetAllAsync(request.PageNumber, request.PageSize, token);
        var dtoItems = _mapper.Map<IEnumerable<AccountDTO>>(result.Items);
        
        return new PaginatedResult<AccountDTO>(dtoItems, result.TotalCount, result.PageNumber, result.PageSize);
    }
}