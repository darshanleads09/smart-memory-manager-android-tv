using SmartMemoryManager.Domain.Models;

namespace SmartMemoryManager.Application.Abstractions;

public interface ISmartMemoryDatabase
{
    Task InsertHealthAsync(DeviceHealth health, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DeviceHealth>> GetRecentHealthAsync(int count, CancellationToken cancellationToken = default);

    Task ReplaceRecommendationsAsync(IReadOnlyList<Recommendation> recommendations, CancellationToken cancellationToken = default);
}
