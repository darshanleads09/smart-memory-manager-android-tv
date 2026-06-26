namespace SmartMemoryManager.Domain.Models;

public sealed class Recommendation
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Priority { get; set; }

    public string Action { get; set; } = string.Empty;
}
