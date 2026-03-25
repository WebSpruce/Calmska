using Calmska.Application.DTO;
using Calmska.Domain.Common;
using MediatR;

namespace Calmska.Application.Features.Types_Mood.Queries;

public record GetAllByArgumentQuery(int? TypeId, string? Type, int? PageNumber, int? PageSize) : IRequest<PaginatedResult<Types_MoodDTO>>;