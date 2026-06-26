using SmartMemoryManager.Application.Abstractions;

namespace SmartMemoryManager.Infrastructure.Cleanup;

public sealed class StaleLogCleanupTask : IOptimizationTask
{
    private readonly CleanupOptions options;

    public StaleLogCleanupTask(CleanupOptions options)
    {
        this.options = options;
    }

    public string Name => "Delete stale logs";

    public Task<long> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(DeleteLogs(options.LogDirectory, cancellationToken));
    }

    private static long DeleteLogs(string root, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(root))
        {
            return 0;
        }

        long recovered = 0;
        var cutoff = DateTimeOffset.UtcNow.AddDays(-14);

        foreach (var file in Directory.EnumerateFiles(root, "*.log", SearchOption.AllDirectories))
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var info = new FileInfo(file);
                if (info.LastWriteTimeUtc > cutoff.UtcDateTime)
                {
                    continue;
                }

                var length = info.Length;
                info.Delete();
                recovered += length;
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        return recovered;
    }
}
