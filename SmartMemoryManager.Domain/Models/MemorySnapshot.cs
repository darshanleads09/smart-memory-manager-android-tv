namespace SmartMemoryManager.Domain.Models;

public sealed record MemorySnapshot(
    long TotalBytes,
    long AvailableBytes,
    long LowMemoryThresholdBytes,
    bool IsLowMemory,
    int RunningProcessCount)
{
    public double UsedRatio => TotalBytes <= 0 ? 0 : 1 - ((double)AvailableBytes / TotalBytes);
}
