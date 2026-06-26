using SmartMemoryManager.Application.Abstractions;
using SmartMemoryManager.Domain.Models;
using SmartMemoryManager.Domain.Policies;

namespace SmartMemoryManager.Application.Services;

public sealed class RecommendationEngine
{
    private readonly RecommendationPolicy policy;
    private readonly IRecommendationStore recommendationStore;

    public RecommendationEngine(RecommendationPolicy policy, IRecommendationStore recommendationStore)
    {
        this.policy = policy;
        this.recommendationStore = recommendationStore;
    }

    public async Task<IReadOnlyList<Recommendation>> GenerateAsync(
        StorageSnapshot storage,
        MemorySnapshot memory,
        UsbDeviceInfo? usbDevice,
        CancellationToken cancellationToken = default)
    {
        var recommendations = policy.Evaluate(storage, memory, usbDevice);
        await recommendationStore.SaveRecommendationsAsync(recommendations, cancellationToken);
        return recommendations;
    }
}
