using SmartMemoryManager.Application.Abstractions;
using SmartMemoryManager.Application.Models;
using SmartMemoryManager.Domain.Models;

namespace SmartMemoryManager.Application.Services;

public sealed class DashboardService
{
    private readonly IDeviceMetricsProvider metricsProvider;
    private readonly IUsbDeviceProvider usbDeviceProvider;
    private readonly RecommendationEngine recommendationEngine;
    private readonly IHealthRepository healthRepository;

    public DashboardService(
        IDeviceMetricsProvider metricsProvider,
        IUsbDeviceProvider usbDeviceProvider,
        RecommendationEngine recommendationEngine,
        IHealthRepository healthRepository)
    {
        this.metricsProvider = metricsProvider;
        this.usbDeviceProvider = usbDeviceProvider;
        this.recommendationEngine = recommendationEngine;
        this.healthRepository = healthRepository;
    }

    public async Task<DashboardSnapshot> ScanAsync(CancellationToken cancellationToken = default)
    {
        var device = await metricsProvider.GetDeviceInfoAsync(cancellationToken);
        var memory = await metricsProvider.GetMemorySnapshotAsync(cancellationToken);
        var storage = await metricsProvider.GetStorageSnapshotAsync(cancellationToken);
        var usbDevice = await usbDeviceProvider.GetAttachedDeviceAsync(cancellationToken);
        var recommendations = await recommendationEngine.GenerateAsync(storage, memory, usbDevice, cancellationToken);

        await healthRepository.SaveDeviceHealthAsync(new DeviceHealth
        {
            Timestamp = DateTimeOffset.UtcNow,
            FreeMemory = memory.AvailableBytes,
            FreeStorage = storage.InternalFreeBytes,
            UsedStorage = storage.InternalUsedBytes
        }, cancellationToken);

        return new DashboardSnapshot(device, memory, storage, usbDevice, recommendations);
    }
}
