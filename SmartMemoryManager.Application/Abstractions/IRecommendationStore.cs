using SmartMemoryManager.Domain.Models;

namespace SmartMemoryManager.Application.Abstractions;

public interface IRecommendationStore
{
    Task SaveRecommendationsAsync(IReadOnlyList<Recommendation> recommendations, CancellationToken cancellationToken = default);
}
