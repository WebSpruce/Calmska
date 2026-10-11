using Calmska.Application.DTO;
using MediatR;

namespace Calmska.Application.Features.Settings.Queries;

public record GetByArgumentQuery(Guid? SettingsId, string? Color, float? PomodoroTimer, float? PomodoroBreak, Guid? UserId) : IRequest<SettingsDTO?>;