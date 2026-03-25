using Calmska.Domain.Common;
using MediatR;

namespace Calmska.Application.Features.Moods.Commands;

public record UpdateCommand(Guid? MoodId, string? MoodName, int? MoodTypeId) : IRequest<OperationResult>;