using SmartMemoryManager.Application.Abstractions;
using SmartMemoryManager.Domain.Models;

namespace SmartMemoryManager.Application.Services;

public sealed class OptimizationEngine
{
    private readonly IEnumerable<IOptimizationTask> tasks;

    public OptimizationEngine(IEnumerable<IOptimizationTask> tasks)
    {
        this.tasks = tasks;
    }

    public async Task<OptimizationResult> OptimizeAsync(CancellationToken cancellationToken = default)
    {
        var completed = new List<string>();
        long recovered = 0;

        foreach (var task in tasks)
        {
            cancellationToken.ThrowIfCancellationRequested();
            recovered += await task.ExecuteAsync(cancellationToken);
            completed.Add(task.Name);
        }

        return new OptimizationResult(DateTimeOffset.UtcNow, recovered, completed);
    }
}
