using Calmska.Domain.Common;
using MediatR;

namespace Calmska.Application.Features.Moods.Commands;

public record CreateCommand(string MoodName, int MoodTypeId) : IRequest<OperationResult>;