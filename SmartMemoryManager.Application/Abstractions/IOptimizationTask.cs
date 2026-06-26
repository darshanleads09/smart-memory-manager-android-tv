namespace SmartMemoryManager.Application.Abstractions;

public interface IOptimizationTask
{
    string Name { get; }

    Task<long> ExecuteAsync(CancellationToken cancellationToken = default);
}
