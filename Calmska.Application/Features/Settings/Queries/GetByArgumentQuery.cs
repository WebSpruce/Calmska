using Calmska.Domain.Entities;
using MediatR;

namespace Calmska.Application.Features.Settings.Queries;

public record GetByArgumentQuery(Guid? SettingsId, string? Color, string? PomodoroTimer, string? PomodoroBreak, Guid? UserId) : IRequest<Domain.Entities.Settings?>;