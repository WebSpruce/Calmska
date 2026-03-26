using Calmska.Domain.Common;
using MediatR;

namespace Calmska.Application.Features.Types_Mood.Commands;

public record DeleteCommand(int Id) : IRequest<OperationResult>;