namespace SmartMemoryManager.Domain.Models;

public sealed record DeviceInfo(
    string Manufacturer,
    string Model,
    long TotalStorage,
    long FreeStorage);
