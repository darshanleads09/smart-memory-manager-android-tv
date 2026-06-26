#if ANDROID
using Android.Hardware.Usb;
using SmartMemoryManager.Application.Abstractions;
using SmartMemoryManager.Domain.Models;

namespace SmartMemoryManager.UI.Platforms.Android;

public sealed class AndroidUsbDeviceProvider : IUsbDeviceProvider
{
    public Task<UsbDeviceInfo?> GetAttachedDeviceAsync(CancellationToken cancellationToken = default)
    {
        var usbManager = Platform.AppContext.GetSystemService(global::Android.Content.Context.UsbService) as UsbManager;
        var device = usbManager?.DeviceList?.Values?.FirstOrDefault();

        if (device is null)
        {
            return Task.FromResult<UsbDeviceInfo?>(null);
        }

        var name = string.IsNullOrWhiteSpace(device.ProductName) ? device.DeviceName : device.ProductName;
        return Task.FromResult<UsbDeviceInfo?>(new UsbDeviceInfo(name ?? "USB device", "Unknown", TryGetExternalCapacity()));
    }

    private static long TryGetExternalCapacity()
    {
        try
        {
            var directories = Platform.AppContext.GetExternalFilesDirs(null) ?? [];
            var externalPath = directories
                .Skip(1)
                .FirstOrDefault(directory => directory is not null)
                ?.AbsolutePath;

            if (string.IsNullOrWhiteSpace(externalPath))
            {
                return 0;
            }

            var stat = new global::Android.OS.StatFs(externalPath);
            return stat.BlockCountLong * stat.BlockSizeLong;
        }
        catch (Exception)
        {
            return 0;
        }
    }
}
#endif
