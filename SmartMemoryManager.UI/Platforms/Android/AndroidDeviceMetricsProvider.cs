#if ANDROID
using Android.App;
using Android.App.Usage;
using Android.Content.PM;
using Android.OS;
using Android.OS.Storage;
using SmartMemoryManager.Application.Abstractions;
using SmartMemoryManager.Domain.Models;
using DomainDeviceInfo = SmartMemoryManager.Domain.Models.DeviceInfo;

namespace SmartMemoryManager.UI.Platforms.Android;

public sealed class AndroidDeviceMetricsProvider : IDeviceMetricsProvider
{
    public Task<DomainDeviceInfo> GetDeviceInfoAsync(CancellationToken cancellationToken = default)
    {
        var statFs = new StatFs(global::Android.OS.Environment.DataDirectory?.AbsolutePath);
        var total = statFs.BlockCountLong * statFs.BlockSizeLong;
        var free = statFs.AvailableBlocksLong * statFs.BlockSizeLong;

        return Task.FromResult(new DomainDeviceInfo(
            Build.Manufacturer ?? "Unknown",
            Build.Model ?? "Android TV",
            total,
            free));
    }

    public Task<MemorySnapshot> GetMemorySnapshotAsync(CancellationToken cancellationToken = default)
    {
        var activityManager = Platform.CurrentActivity?.GetSystemService(global::Android.Content.Context.ActivityService) as ActivityManager;
        var memoryInfo = new ActivityManager.MemoryInfo();
        activityManager?.GetMemoryInfo(memoryInfo);

        return Task.FromResult(new MemorySnapshot(
            memoryInfo.TotalMem,
            memoryInfo.AvailMem,
            memoryInfo.Threshold,
            memoryInfo.LowMemory,
            GetRunningProcessCount(activityManager)));
    }

    public Task<StorageSnapshot> GetStorageSnapshotAsync(CancellationToken cancellationToken = default)
    {
        var internalStat = new StatFs(global::Android.OS.Environment.DataDirectory?.AbsolutePath);
        var internalTotal = internalStat.BlockCountLong * internalStat.BlockSizeLong;
        var internalFree = internalStat.AvailableBlocksLong * internalStat.BlockSizeLong;
        var cacheBytes = GetDirectorySize(FileSystem.CacheDirectory);
        var externalPath = Platform.AppContext.GetExternalFilesDir(null)?.AbsolutePath;
        var externalStat = string.IsNullOrWhiteSpace(externalPath) ? null : new StatFs(externalPath);
        var externalTotal = externalStat is null ? 0 : externalStat.BlockCountLong * externalStat.BlockSizeLong;
        var externalFree = externalStat is null ? 0 : externalStat.AvailableBlocksLong * externalStat.BlockSizeLong;

        var apps = GetInstalledApps()
            .OrderByDescending(app => app.AppBytes + app.CacheBytes)
            .Take(10)
            .ToArray();

        return Task.FromResult(new StorageSnapshot(
            internalTotal,
            internalFree,
            externalTotal,
            externalFree,
            cacheBytes,
            apps));
    }

    private static int GetRunningProcessCount(ActivityManager? activityManager)
    {
        if (activityManager?.RunningAppProcesses is null)
        {
            return 0;
        }

        return activityManager.RunningAppProcesses.Count;
    }

    private static IReadOnlyList<AppUsageInfo> GetInstalledApps()
    {
        var packageManager = Platform.AppContext.PackageManager;
        if (packageManager is null)
        {
            return [];
        }

        var packages = packageManager.GetInstalledPackages(PackageInfoFlags.MetaData);
        var storageStatsManager = Platform.AppContext.GetSystemService(global::Android.Content.Context.StorageStatsService) as StorageStatsManager;
        var apps = new List<AppUsageInfo>();

        foreach (var package in packages)
        {
            var appInfo = package.ApplicationInfo;
            if (appInfo is null)
            {
                continue;
            }

            var label = packageManager.GetApplicationLabel(appInfo)?.ToString() ?? package.PackageName ?? "Unknown";
            var sourceSize = GetFileSize(appInfo.SourceDir);
            var stats = TryGetPackageStats(storageStatsManager, package.PackageName);
            apps.Add(new AppUsageInfo(package.PackageName ?? label, label, Math.Max(sourceSize, stats.AppBytes), stats.CacheBytes, IsMovable(appInfo)));
        }

        return apps;
    }

    private static (long AppBytes, long CacheBytes) TryGetPackageStats(StorageStatsManager? storageStatsManager, string? packageName)
    {
        if (storageStatsManager is null || string.IsNullOrWhiteSpace(packageName) || Build.VERSION.SdkInt < BuildVersionCodes.O)
        {
            return (0, 0);
        }

        try
        {
            var storageUuid = StorageManager.UuidDefault;
            var user = global::Android.OS.Process.MyUserHandle();
            if (storageUuid is null || user is null)
            {
                return (0, 0);
            }

            var stats = storageStatsManager.QueryStatsForPackage(storageUuid, packageName, user);
            return (stats.AppBytes + stats.DataBytes, stats.CacheBytes);
        }
        catch (Exception)
        {
            return (0, 0);
        }
    }

    private static bool IsMovable(ApplicationInfo appInfo)
    {
        return (appInfo.Flags & ApplicationInfoFlags.ExternalStorage) == ApplicationInfoFlags.ExternalStorage;
    }

    private static long GetDirectorySize(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
        {
            return 0;
        }

        long size = 0;
        foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
        {
            try
            {
                size += new FileInfo(file).Length;
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        return size;
    }

    private static long GetFileSize(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            return 0;
        }

        try
        {
            return new FileInfo(path).Length;
        }
        catch (IOException)
        {
            return 0;
        }
        catch (UnauthorizedAccessException)
        {
            return 0;
        }
    }
}
#endif
