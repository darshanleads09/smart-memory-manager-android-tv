using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SmartMemoryManager.Application.Services;
using SmartMemoryManager.Domain;

namespace SmartMemoryManager.UI.ViewModels;

public sealed class DashboardViewModel : INotifyPropertyChanged
{
    private readonly DashboardService dashboardService;
    private readonly OptimizationEngine optimizationEngine;
    private bool isBusy;
    private string status = "Ready";
    private string deviceName = "Android TV";
    private string lastScan = "Not scanned";
    private string dailyMaintenance = "Daily scan pending";
    private string weeklyMaintenance = "Weekly cleanup pending";
    private string monthlyMaintenance = "Monthly assessment pending";
    private string storageSummary = "Storage analysis not available";
    private string memorySummary = "Memory analysis not available";
    private readonly MaintenanceScheduler maintenanceScheduler;

    public DashboardViewModel(
        DashboardService dashboardService,
        OptimizationEngine optimizationEngine,
        MaintenanceScheduler maintenanceScheduler)
    {
        this.dashboardService = dashboardService;
        this.optimizationEngine = optimizationEngine;
        this.maintenanceScheduler = maintenanceScheduler;
        ScanCommand = new Command(async () => await ScanAsync(), () => !IsBusy);
        OptimizeCommand = new Command(async () => await OptimizeAsync(), () => !IsBusy);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<MetricCardViewModel> Metrics { get; } = [];

    public ObservableCollection<RecommendationViewModel> Recommendations { get; } = [];

    public ObservableCollection<AppUsageViewModel> TopApps { get; } = [];

    public ICommand ScanCommand { get; }

    public ICommand OptimizeCommand { get; }

    public bool IsBusy
    {
        get => isBusy;
        private set
        {
            if (SetField(ref isBusy, value))
            {
                ((Command)ScanCommand).ChangeCanExecute();
                ((Command)OptimizeCommand).ChangeCanExecute();
            }
        }
    }

    public string Status
    {
        get => status;
        private set => SetField(ref status, value);
    }

    public string DeviceName
    {
        get => deviceName;
        private set => SetField(ref deviceName, value);
    }

    public string LastScan
    {
        get => lastScan;
        private set => SetField(ref lastScan, value);
    }

    public string DailyMaintenance
    {
        get => dailyMaintenance;
        private set => SetField(ref dailyMaintenance, value);
    }

    public string WeeklyMaintenance
    {
        get => weeklyMaintenance;
        private set => SetField(ref weeklyMaintenance, value);
    }

    public string MonthlyMaintenance
    {
        get => monthlyMaintenance;
        private set => SetField(ref monthlyMaintenance, value);
    }

    public string StorageSummary
    {
        get => storageSummary;
        private set => SetField(ref storageSummary, value);
    }

    public string MemorySummary
    {
        get => memorySummary;
        private set => SetField(ref memorySummary, value);
    }

    public async Task ScanAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        Status = "Scanning device";

        try
        {
            var snapshot = await dashboardService.ScanAsync();
            DeviceName = $"{snapshot.Device.Manufacturer} {snapshot.Device.Model}".Trim();
            var now = DateTimeOffset.Now;
            LastScan = now.ToString("g");
            DailyMaintenance = $"Daily scan: {maintenanceScheduler.GetNextDailyScan(now):g}";
            WeeklyMaintenance = $"Weekly cleanup: {maintenanceScheduler.GetNextWeeklyCleanup(now):g}";
            MonthlyMaintenance = $"Monthly health: {maintenanceScheduler.GetNextMonthlyAssessment(now):g}";
            StorageSummary = $"{ByteSize.Format(snapshot.Storage.InternalUsedBytes)} used of {ByteSize.Format(snapshot.Storage.InternalTotalBytes)}";
            MemorySummary = $"{ByteSize.Format(snapshot.Memory.AvailableBytes)} available of {ByteSize.Format(snapshot.Memory.TotalBytes)}";

            Metrics.Clear();
            Metrics.Add(new MetricCardViewModel
            {
                Title = "Free Storage",
                Value = ByteSize.Format(snapshot.Storage.InternalFreeBytes),
                Detail = $"{snapshot.Storage.InternalUsedRatio:P0} used"
            });
            Metrics.Add(new MetricCardViewModel
            {
                Title = "RAM Available",
                Value = ByteSize.Format(snapshot.Memory.AvailableBytes),
                Detail = snapshot.Memory.IsLowMemory ? "Low memory" : $"{snapshot.Memory.RunningProcessCount} processes"
            });
            Metrics.Add(new MetricCardViewModel
            {
                Title = "Cache Usage",
                Value = ByteSize.Format(snapshot.Storage.CacheBytes),
                Detail = "Detected app-owned and visible cache"
            });
            Metrics.Add(new MetricCardViewModel
            {
                Title = "USB Status",
                Value = snapshot.UsbDevice?.Name ?? "Not detected",
                Detail = snapshot.UsbDevice is null ? "Attach USB to expand storage" : ByteSize.Format(snapshot.UsbDevice.Capacity)
            });

            Recommendations.Clear();
            foreach (var recommendation in snapshot.Recommendations)
            {
                Recommendations.Add(new RecommendationViewModel
                {
                    Title = recommendation.Title,
                    Description = recommendation.Description,
                    Priority = $"P{recommendation.Priority}"
                });
            }

            TopApps.Clear();
            foreach (var app in snapshot.Storage.LargestApps.Take(10))
            {
                TopApps.Add(new AppUsageViewModel
                {
                    Name = app.DisplayName,
                    Size = ByteSize.Format(app.AppBytes),
                    Cache = ByteSize.Format(app.CacheBytes),
                    MigrationStatus = app.IsMovable ? "Movable" : "Internal"
                });
            }

            Status = $"Scan complete: {Recommendations.Count} recommendations";
        }
        catch (Exception ex)
        {
            Status = $"Scan failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task OptimizeAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        Status = "Optimizing";

        try
        {
            var result = await optimizationEngine.OptimizeAsync();
            Status = $"Recovered {ByteSize.Format(result.BytesRecovered)} across {result.CompletedTasks.Count} tasks";
        }
        catch (Exception ex)
        {
            Status = $"Optimization failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
