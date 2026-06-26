namespace SmartMemoryManager.UI.ViewModels;

public sealed class AppUsageViewModel
{
    public string Name { get; init; } = string.Empty;

    public string Size { get; init; } = string.Empty;

    public string Cache { get; init; } = string.Empty;

    public string MigrationStatus { get; init; } = string.Empty;
}
