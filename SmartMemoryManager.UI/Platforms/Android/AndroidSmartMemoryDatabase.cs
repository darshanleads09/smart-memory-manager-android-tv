#if ANDROID
using Android.Content;
using Android.Database.Sqlite;
using SmartMemoryManager.Application.Abstractions;
using SmartMemoryManager.Domain.Models;

namespace SmartMemoryManager.UI.Platforms.Android;

public sealed class AndroidSmartMemoryDatabase : SQLiteOpenHelper, ISmartMemoryDatabase
{
    private const string SmartMemoryDatabaseName = "smart-memory-manager.db";
    private const int DatabaseVersion = 1;
    private readonly SemaphoreSlim gate = new(1, 1);

    public AndroidSmartMemoryDatabase()
        : base(Platform.AppContext, SmartMemoryDatabaseName, null, DatabaseVersion)
    {
    }

    public override void OnCreate(SQLiteDatabase? db)
    {
        db?.ExecSQL(
            """
            CREATE TABLE IF NOT EXISTS DeviceHealth
            (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Timestamp TEXT NOT NULL,
                FreeStorage INTEGER NOT NULL,
                UsedStorage INTEGER NOT NULL,
                FreeMemory INTEGER NOT NULL
            )
            """);

        db?.ExecSQL(
            """
            CREATE TABLE IF NOT EXISTS Recommendations
            (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Description TEXT NOT NULL,
                Priority INTEGER NOT NULL,
                Action TEXT NOT NULL,
                CreatedAt TEXT NOT NULL
            )
            """);

        db?.ExecSQL("CREATE INDEX IF NOT EXISTS IX_DeviceHealth_Timestamp ON DeviceHealth(Timestamp)");
        db?.ExecSQL("CREATE INDEX IF NOT EXISTS IX_Recommendations_Priority ON Recommendations(Priority)");
    }

    public override void OnUpgrade(SQLiteDatabase? db, int oldVersion, int newVersion)
    {
        OnCreate(db);
    }

    public async Task InsertHealthAsync(DeviceHealth health, CancellationToken cancellationToken = default)
    {
        await gate.WaitAsync(cancellationToken);
        try
        {
            using var database = WritableDatabase ?? throw new InvalidOperationException("SQLite database is not writable.");
            var values = new ContentValues();
            values.Put("Timestamp", health.Timestamp.UtcDateTime.ToString("O"));
            values.Put("FreeStorage", health.FreeStorage);
            values.Put("UsedStorage", health.UsedStorage);
            values.Put("FreeMemory", health.FreeMemory);
            database.Insert("DeviceHealth", null, values);
            database.ExecSQL("DELETE FROM DeviceHealth WHERE Id NOT IN (SELECT Id FROM DeviceHealth ORDER BY Timestamp DESC LIMIT 500)");
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task<IReadOnlyList<DeviceHealth>> GetRecentHealthAsync(int count, CancellationToken cancellationToken = default)
    {
        await gate.WaitAsync(cancellationToken);
        try
        {
            using var database = ReadableDatabase ?? throw new InvalidOperationException("SQLite database is not readable.");
            using var cursor = database.RawQuery(
                "SELECT Id, Timestamp, FreeStorage, UsedStorage, FreeMemory FROM DeviceHealth ORDER BY Timestamp DESC LIMIT ?",
                [Math.Max(0, count).ToString()]);

            var results = new List<DeviceHealth>();
            while (cursor is not null && cursor.MoveToNext())
            {
                results.Add(new DeviceHealth
                {
                    Id = cursor.GetInt(0),
                    Timestamp = DateTimeOffset.Parse(cursor.GetString(1) ?? DateTimeOffset.UtcNow.ToString("O")),
                    FreeStorage = cursor.GetLong(2),
                    UsedStorage = cursor.GetLong(3),
                    FreeMemory = cursor.GetLong(4)
                });
            }

            return results;
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task ReplaceRecommendationsAsync(IReadOnlyList<Recommendation> recommendations, CancellationToken cancellationToken = default)
    {
        await gate.WaitAsync(cancellationToken);
        try
        {
            using var database = WritableDatabase ?? throw new InvalidOperationException("SQLite database is not writable.");
            database.BeginTransaction();
            try
            {
                database.Delete("Recommendations", null, null);
                foreach (var recommendation in recommendations)
                {
                    var values = new ContentValues();
                    values.Put("Title", recommendation.Title);
                    values.Put("Description", recommendation.Description);
                    values.Put("Priority", recommendation.Priority);
                    values.Put("Action", recommendation.Action);
                    values.Put("CreatedAt", DateTime.UtcNow.ToString("O"));
                    database.Insert("Recommendations", null, values);
                }

                database.SetTransactionSuccessful();
            }
            finally
            {
                database.EndTransaction();
            }
        }
        finally
        {
            gate.Release();
        }
    }
}
#endif
