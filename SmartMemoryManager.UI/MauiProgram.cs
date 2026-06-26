using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using SmartMemoryManager.Application;
using SmartMemoryManager.Application.Abstractions;
using SmartMemoryManager.Infrastructure.Cleanup;
using SmartMemoryManager.Infrastructure.Persistence;
using SmartMemoryManager.UI.ViewModels;
#if ANDROID
using SmartMemoryManager.UI.Platforms.Android;
#endif

namespace SmartMemoryManager.UI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddApplication();
        AddInfrastructure(builder.Services);
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<DashboardViewModel>();

#if ANDROID
        builder.Services.AddSingleton<IDeviceMetricsProvider, AndroidDeviceMetricsProvider>();
        builder.Services.AddSingleton<IUsbDeviceProvider, AndroidUsbDeviceProvider>();
        builder.Services.AddSingleton<ISmartMemoryDatabase, AndroidSmartMemoryDatabase>();
#endif

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static void AddInfrastructure(IServiceCollection services)
    {
        var tempDirectory = Path.Combine(FileSystem.AppDataDirectory, "temp");
        var logDirectory = Path.Combine(FileSystem.AppDataDirectory, "logs");
        Directory.CreateDirectory(tempDirectory);
        Directory.CreateDirectory(logDirectory);

        services.AddSingleton(new CleanupOptions(tempDirectory, logDirectory));
        services.AddSingleton<IHealthRepository, SmartMemoryHealthRepository>();
        services.AddSingleton<IRecommendationStore, SmartMemoryRecommendationStore>();
        services.AddSingleton<IOptimizationTask, AppTempCleanupTask>();
        services.AddSingleton<IOptimizationTask, StaleLogCleanupTask>();
    }
}
