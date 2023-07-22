using SQLite;

namespace TimeTracker.Database.Migrations;

public class M005_AddTimestampAndReworkElapsed : IDbMigration
{
    public async Task Do(SQLiteAsyncConnection db)
    {
        var timestamp = DateTime.Now;

        await db.ExecuteAsync("ALTER TABLE TrackedTimeDb ADD Timestamp datetime");
        await db.ExecuteAsync("ALTER TABLE TrackedTimeDb ADD EndTime datetime");

        var all = await db.Table<TrackedTimeDb_M005>().ToListAsync();
        foreach (var trackedTimeDb in all)
        {
            trackedTimeDb.Timestamp = timestamp;
#pragma warning disable CS0618
            trackedTimeDb.EndTime = trackedTimeDb.StartTime + trackedTimeDb.ElapsedTime;
#pragma warning restore CS0618
            await db.UpdateAsync(trackedTimeDb);
        }

        // NOTE: sqlite does not support DROP COLUMN
        // await db.ExecuteAsync("ALTER TABLE TrackedTimeDb DROP COLUMN ElapsedTime");

        const string TemporaryTable = "t1_backup";
        await db.ExecuteAsync(
            $"CREATE TABLE {TemporaryTable} AS SELECT Id, Uuid, Name, Status, StartTime, EndTime, Timestamp FROM TrackedTimeDb");
        await db.ExecuteAsync("DROP TABLE TrackedTimeDb");
        await db.ExecuteAsync($"ALTER TABLE {TemporaryTable} RENAME TO TrackedTimeDb");
    }

    public Task UnDo(SQLiteAsyncConnection db)
    {
        throw new NotImplementedException();
    }

    public string Serialize()
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Data model at the time of migration <see cref="M005_AddTimestampAndReworkElapsed"/>
/// </summary>
[Table(nameof(TrackedTimeDb))]
public class TrackedTimeDb_M005 : ITable
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public Guid Uuid { get; set; }

    public string Name { get; set; }

    public DateTime StartTime { get; set; }

    public TimeSpan ElapsedTime { get; init; }
    public DateTime EndTime { get; set; }

    public int Status { get; set; }

    /// <summary>
    /// Last modification time - considering Start and Elapsed may ba changed later
    /// </summary>
    public DateTime Timestamp { get; set; }

    public enum TrackingStatus
    {
        Completed = 0,

        /// <summary>
        /// Currently running
        /// </summary>
        Running = 1,
    }
}
