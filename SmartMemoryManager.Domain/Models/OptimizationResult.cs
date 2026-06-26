namespace SmartMemoryManager.Domain.Models;

public sealed record OptimizationResult(
    DateTimeOffset CompletedAt,
    long BytesRecovered,
    IReadOnlyList<string> CompletedTasks);
