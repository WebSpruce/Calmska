using Calmska.Application.DTO;
using Calmska.Domain.Common;
using MediatR;

namespace Calmska.Application.Features.Settings.Queries;

public record GetAllByArgumentQuery(Guid? SettingsId, string? Color, string? PomodoroTimer, string? PomodoroBreak, Guid? UserId, int? PageNumber, int? PageSize) : IRequest<PaginatedResult<SettingsDTO>>;