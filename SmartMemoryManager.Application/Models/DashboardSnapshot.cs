using SmartMemoryManager.Domain.Models;

namespace SmartMemoryManager.Application.Models;

public sealed record DashboardSnapshot(
    DeviceInfo Device,
    MemorySnapshot Memory,
    StorageSnapshot Storage,
    UsbDeviceInfo? UsbDevice,
    IReadOnlyList<Recommendation> Recommendations);
