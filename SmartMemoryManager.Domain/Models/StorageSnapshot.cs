namespace SmartMemoryManager.Domain.Models;

public sealed record StorageSnapshot(
    long InternalTotalBytes,
    long InternalFreeBytes,
    long ExternalTotalBytes,
    long ExternalFreeBytes,
    long CacheBytes,
    IReadOnlyList<AppUsageInfo> LargestApps)
{
    public long InternalUsedBytes => Math.Max(0, InternalTotalBytes - InternalFreeBytes);

    public double InternalUsedRatio => InternalTotalBytes <= 0 ? 0 : (double)InternalUsedBytes / InternalTotalBytes;
}
