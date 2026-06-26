namespace SmartMemoryManager.Domain.Models;

public sealed record UsbBenchmarkResult(
    double SequentialReadMbps,
    double SequentialWriteMbps,
    double RandomReadIops,
    double RandomWriteIops);
