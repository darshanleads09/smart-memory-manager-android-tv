using SmartMemoryManager.Application.Abstractions;
using SmartMemoryManager.Domain.Models;

namespace SmartMemoryManager.Infrastructure.Persistence;

public sealed class SmartMemoryHealthRepository : IHealthRepository
{
    private readonly ISmartMemoryDatabase database;

    public SmartMemoryHealthRepository(ISmartMemoryDatabase database)
    {
        this.database = database;
    }

    public Task SaveDeviceHealthAsync(DeviceHealth health, CancellationToken cancellationToken = default)
    {
        return database.InsertHealthAsync(health, cancellationToken);
    }

    public Task<IReadOnlyList<DeviceHealth>> GetRecentHealthAsync(int count, CancellationToken cancellationToken = default)
    {
        return database.GetRecentHealthAsync(count, cancellationToken);
    }
}
