namespace SmartMemoryManager.Domain.Models;

public sealed class DeviceHealth
{
    public int Id { get; set; }

    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    public long FreeStorage { get; set; }

    public long UsedStorage { get; set; }

    public long FreeMemory { get; set; }
}
