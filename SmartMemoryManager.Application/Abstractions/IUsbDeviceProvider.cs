using SmartMemoryManager.Domain.Models;

namespace SmartMemoryManager.Application.Abstractions;

public interface IUsbDeviceProvider
{
    Task<UsbDeviceInfo?> GetAttachedDeviceAsync(CancellationToken cancellationToken = default);
}
