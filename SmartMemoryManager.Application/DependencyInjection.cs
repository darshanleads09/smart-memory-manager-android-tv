using Microsoft.Extensions.DependencyInjection;
using SmartMemoryManager.Application.Services;
using SmartMemoryManager.Domain.Policies;

namespace SmartMemoryManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<RecommendationPolicy>();
        services.AddSingleton<RecommendationEngine>();
        services.AddSingleton<DashboardService>();
        services.AddSingleton<OptimizationEngine>();
        services.AddSingleton<MaintenanceScheduler>();

        return services;
    }
}
