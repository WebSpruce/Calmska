using Calmska.Domain.Entities;
using MediatR;

namespace Calmska.Application.Features.Moods.Queries;

public record GetByArgumentQuery(Guid? MoodId, string? MoodName, int? MoodTypeId) : IRequest<Mood?>;