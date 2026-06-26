namespace SmartMemoryManager.Domain.Models;

public sealed record UsbDeviceInfo(
    string Name,
    string FileSystem,
    long Capacity,
    UsbBenchmarkResult? Benchmark = null);
