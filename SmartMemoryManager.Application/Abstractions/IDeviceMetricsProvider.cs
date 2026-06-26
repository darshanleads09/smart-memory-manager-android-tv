using SmartMemoryManager.Domain.Models;

namespace SmartMemoryManager.Application.Abstractions;

public interface IDeviceMetricsProvider
{
    Task<DeviceInfo> GetDeviceInfoAsync(CancellationToken cancellationToken = default);

    Task<MemorySnapshot> GetMemorySnapshotAsync(CancellationToken cancellationToken = default);

    Task<StorageSnapshot> GetStorageSnapshotAsync(CancellationToken cancellationToken = default);
}
