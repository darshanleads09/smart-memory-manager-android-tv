using SmartMemoryManager.Application.Services;

namespace SmartMemoryManager.Tests;

[TestClass]
public sealed class MaintenanceSchedulerTests
{
    [TestMethod]
    public void GetNextWeeklyCleanup_WhenTodayIsSunday_ReturnsFollowingSunday()
    {
        var scheduler = new MaintenanceScheduler();
        var now = new DateTimeOffset(2026, 6, 7, 14, 0, 0, TimeSpan.Zero);

        var next = scheduler.GetNextWeeklyCleanup(now);

        Assert.AreEqual(new DateTimeOffset(2026, 6, 14, 2, 0, 0, TimeSpan.Zero), next);
    }

    [TestMethod]
    public void GetNextMonthlyAssessment_ReturnsFirstDayOfNextMonthAtThree()
    {
        var scheduler = new MaintenanceScheduler();
        var now = new DateTimeOffset(2026, 6, 7, 14, 0, 0, TimeSpan.FromHours(5.5));

        var next = scheduler.GetNextMonthlyAssessment(now);

        Assert.AreEqual(new DateTimeOffset(2026, 7, 1, 3, 0, 0, TimeSpan.FromHours(5.5)), next);
    }
}
