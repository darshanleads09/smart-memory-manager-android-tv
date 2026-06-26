using SmartMemoryManager.Domain.Models;

namespace SmartMemoryManager.Application.Abstractions;

public interface IHealthRepository
{
    Task SaveDeviceHealthAsync(DeviceHealth health, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DeviceHealth>> GetRecentHealthAsync(int count, CancellationToken cancellationToken = default);
}
