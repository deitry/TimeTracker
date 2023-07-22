using SQLite;

namespace TimeTracker.Database.Migrations;

public class M4_AddUuid : IDbMigration
{
    public async Task Do(SQLiteAsyncConnection db)
    {
        await db.ExecuteAsync("ALTER TABLE TrackedTimeDb ADD Uuid text");

        var all = await db.Table<TrackedTimeDb_M004>().ToListAsync();
        foreach (var trackedTimeDb in all)
        {
            trackedTimeDb.Uuid = Guid.NewGuid();
            await db.UpdateAsync(trackedTimeDb);
        }
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
/// Data model at the time of migration <see cref="M4_AddUuid"/>
/// </summary>
[Table(nameof(TrackedTimeDb))]
public class TrackedTimeDb_M004 : ITable
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public Guid Uuid { get; set; }

    public string Name { get; set; }

    public DateTime StartTime { get; set; }

    public TimeSpan ElapsedTime { get; init; }

    public int Status { get; set; }

    public enum TrackingStatus
    {
        Completed = 0,

        /// <summary>
        /// Currently running
        /// </summary>
        Running = 1,
    }
}
