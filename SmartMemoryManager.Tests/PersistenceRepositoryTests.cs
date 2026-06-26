using SmartMemoryManager.Application.Abstractions;
using SmartMemoryManager.Domain.Models;
using SmartMemoryManager.Infrastructure.Persistence;

namespace SmartMemoryManager.Tests;

[TestClass]
public sealed class PersistenceRepositoryTests
{
    [TestMethod]
    public async Task HealthRepository_DelegatesToSmartMemoryDatabase()
    {
        var database = new FakeSmartMemoryDatabase();
        var repository = new SmartMemoryHealthRepository(database);

        await repository.SaveDeviceHealthAsync(new DeviceHealth
        {
            Timestamp = new DateTimeOffset(2026, 6, 21, 10, 0, 0, TimeSpan.Zero),
            FreeStorage = 100,
            UsedStorage = 900,
            FreeMemory = 200
        });

        var recent = await repository.GetRecentHealthAsync(1);

        Assert.AreEqual(1, recent.Count);
        Assert.AreEqual(100, recent[0].FreeStorage);
        Assert.AreEqual(900, recent[0].UsedStorage);
        Assert.AreEqual(200, recent[0].FreeMemory);
    }

    [TestMethod]
    public async Task RecommendationStore_ReplacesPreviousRecommendations()
    {
        var database = new FakeSmartMemoryDatabase();
        var store = new SmartMemoryRecommendationStore(database);

        await store.SaveRecommendationsAsync([Recommendation("Old", 1)]);
        await store.SaveRecommendationsAsync([Recommendation("New", 2), Recommendation("Newer", 3)]);

        Assert.AreEqual(2, database.Recommendations.Count);
        Assert.AreEqual("New", database.Recommendations[0].Title);
    }

    private static Recommendation Recommendation(string title, int priority)
    {
        return new Recommendation
        {
            Title = title,
            Description = title,
            Priority = priority,
            Action = "Test"
        };
    }

    private sealed class FakeSmartMemoryDatabase : ISmartMemoryDatabase
    {
        private readonly List<DeviceHealth> health = [];

        public IReadOnlyList<Recommendation> Recommendations { get; private set; } = [];

        public Task InsertHealthAsync(DeviceHealth entry, CancellationToken cancellationToken = default)
        {
            health.Add(entry);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<DeviceHealth>> GetRecentHealthAsync(int count, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<DeviceHealth>>(health
                .OrderByDescending(entry => entry.Timestamp)
                .Take(count)
                .ToArray());
        }

        public Task ReplaceRecommendationsAsync(IReadOnlyList<Recommendation> recommendations, CancellationToken cancellationToken = default)
        {
            Recommendations = recommendations.ToArray();
            return Task.CompletedTask;
        }
    }
}
