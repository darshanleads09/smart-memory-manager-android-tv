using SmartMemoryManager.Application.Abstractions;
using SmartMemoryManager.Domain.Models;

namespace SmartMemoryManager.Infrastructure.Persistence;

public sealed class SmartMemoryRecommendationStore : IRecommendationStore
{
    private readonly ISmartMemoryDatabase database;

    public SmartMemoryRecommendationStore(ISmartMemoryDatabase database)
    {
        this.database = database;
    }

    public Task SaveRecommendationsAsync(IReadOnlyList<Recommendation> recommendations, CancellationToken cancellationToken = default)
    {
        return database.ReplaceRecommendationsAsync(recommendations, cancellationToken);
    }
}
