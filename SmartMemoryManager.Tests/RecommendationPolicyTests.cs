using SmartMemoryManager.Domain.Models;
using SmartMemoryManager.Domain.Policies;

namespace SmartMemoryManager.Tests;

[TestClass]
public sealed class RecommendationPolicyTests
{
    [TestMethod]
    public void Evaluate_WhenCacheExceedsThreshold_RecommendsCleanup()
    {
        var policy = new RecommendationPolicy();

        var recommendations = policy.Evaluate(
            Storage(cacheBytes: 600L * 1024 * 1024),
            Memory(),
            usbDevice: null);

        Assert.IsTrue(recommendations.Any(recommendation => recommendation.Action == "Cleanup"));
    }

    [TestMethod]
    public void Evaluate_WhenStorageIsCritical_PrioritizesAppMigration()
    {
        var policy = new RecommendationPolicy();

        var recommendations = policy.Evaluate(
            Storage(totalBytes: 1000, freeBytes: 50),
            Memory(),
            usbDevice: null);

        Assert.AreEqual("AppMigration", recommendations[0].Action);
    }

    [TestMethod]
    public void Evaluate_WhenUsbIsAttached_RecommendsExpansion()
    {
        var policy = new RecommendationPolicy();

        var recommendations = policy.Evaluate(
            Storage(),
            Memory(),
            new UsbDeviceInfo("SanDisk", "exFAT", 128L * 1024 * 1024 * 1024));

        Assert.IsTrue(recommendations.Any(recommendation => recommendation.Action == "UsbManager"));
    }

    private static StorageSnapshot Storage(
        long totalBytes = 1000,
        long freeBytes = 500,
        long cacheBytes = 0)
    {
        return new StorageSnapshot(totalBytes, freeBytes, 0, 0, cacheBytes, []);
    }

    private static MemorySnapshot Memory()
    {
        return new MemorySnapshot(1000, 600, 100, false, 4);
    }
}
