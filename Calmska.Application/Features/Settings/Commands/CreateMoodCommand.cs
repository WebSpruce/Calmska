using Calmska.Domain.Common;
using MediatR;

namespace Calmska.Application.Features.Settings.Commands;

public record CreateMoodCommand(Guid SettingsId, string Color, string PomodoroTimer, string PomodoroBreak, Guid UserId) : IRequest<OperationResult>;