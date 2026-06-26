using SmartMemoryManager.Application.Abstractions;

namespace SmartMemoryManager.Infrastructure.Cleanup;

public sealed class AppTempCleanupTask : IOptimizationTask
{
    private readonly CleanupOptions options;

    public AppTempCleanupTask(CleanupOptions options)
    {
        this.options = options;
    }

    public string Name => "Clear app temporary files";

    public Task<long> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(DeleteFilesOlderThan(options.TempDirectory, TimeSpan.FromHours(12), cancellationToken));
    }

    private static long DeleteFilesOlderThan(string root, TimeSpan age, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(root))
        {
            return 0;
        }

        long recovered = 0;
        var cutoff = DateTimeOffset.UtcNow.Subtract(age);

        foreach (var file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
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
