namespace SmartMemoryManager.Domain.Models;

public sealed record AppUsageInfo(
    string PackageName,
    string DisplayName,
    long AppBytes,
    long CacheBytes,
    bool IsMovable);
