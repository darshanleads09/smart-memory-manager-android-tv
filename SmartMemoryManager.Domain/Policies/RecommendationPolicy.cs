using SmartMemoryManager.Domain.Models;

namespace SmartMemoryManager.Domain.Policies;

public sealed class RecommendationPolicy
{
    public const long LargeCacheThresholdBytes = 500L * 1024 * 1024;
    public const double StorageCriticalRatio = 0.90;
    public const double StorageWarningRatio = 0.85;

    public IReadOnlyList<Recommendation> Evaluate(
        StorageSnapshot storage,
        MemorySnapshot memory,
        UsbDeviceInfo? usbDevice)
    {
        var recommendations = new List<Recommendation>();

        if (storage.CacheBytes > LargeCacheThresholdBytes)
        {
            recommendations.Add(new Recommendation
            {
                Title = "Clear app cache",
                Description = $"Cached data is using {ByteSize.Format(storage.CacheBytes)}.",
                Priority = 90,
                Action = "Cleanup"
            });
        }

        if (storage.InternalUsedRatio >= StorageCriticalRatio)
        {
            recommendations.Add(new Recommendation
            {
                Title = "Move large apps to external storage",
                Description = $"Internal storage is {storage.InternalUsedRatio:P0} full.",
                Priority = 100,
                Action = "AppMigration"
            });
        }
        else if (storage.InternalUsedRatio >= StorageWarningRatio)
        {
            recommendations.Add(new Recommendation
            {
                Title = "Review storage usage",
                Description = $"Internal storage is {storage.InternalUsedRatio:P0} full.",
                Priority = 75,
                Action = "StorageAnalysis"
            });
        }

        if (memory.IsLowMemory || memory.AvailableBytes <= memory.LowMemoryThresholdBytes)
        {
            recommendations.Add(new Recommendation
            {
                Title = "Close background apps",
                Description = $"{memory.RunningProcessCount} processes are active and available RAM is low.",
                Priority = 80,
                Action = "MemoryAnalysis"
            });
        }

        if (usbDevice is not null)
        {
            recommendations.Add(new Recommendation
            {
                Title = "Use USB storage expansion",
                Description = $"{usbDevice.Name} is available with {ByteSize.Format(usbDevice.Capacity)} capacity.",
                Priority = 70,
                Action = "UsbManager"
            });
        }

        return recommendations
            .OrderByDescending(recommendation => recommendation.Priority)
            .ToArray();
    }
}
