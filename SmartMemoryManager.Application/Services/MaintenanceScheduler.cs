namespace SmartMemoryManager.Application.Services;

public sealed class MaintenanceScheduler
{
    public DateTimeOffset GetNextDailyScan(DateTimeOffset now) => StartOfDay(now).AddDays(1);

    public DateTimeOffset GetNextWeeklyCleanup(DateTimeOffset now)
    {
        var daysUntilSunday = ((int)DayOfWeek.Sunday - (int)now.DayOfWeek + 7) % 7;
        if (daysUntilSunday == 0)
        {
            daysUntilSunday = 7;
        }

        return StartOfDay(now).AddDays(daysUntilSunday).AddHours(2);
    }

    public DateTimeOffset GetNextMonthlyAssessment(DateTimeOffset now)
    {
        var firstOfNextMonth = new DateTimeOffset(now.Year, now.Month, 1, 3, 0, 0, now.Offset).AddMonths(1);
        return firstOfNextMonth;
    }

    private static DateTimeOffset StartOfDay(DateTimeOffset value)
    {
        return new DateTimeOffset(value.Year, value.Month, value.Day, 0, 0, 0, value.Offset);
    }
}
