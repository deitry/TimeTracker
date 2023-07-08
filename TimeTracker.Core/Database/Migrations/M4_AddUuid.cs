using SQLite;

namespace TimeTracker.Database.Migrations;

public class M4_AddUuid : IDbMigration
{
    public Task Do(SQLiteAsyncConnection db)
    {
        // iterate over TrackedTimeDb and add Guid.NewGuid() as parameter
        return db.ExecuteAsync("ALTER TABLE TrackedTimeDb ADD Uuid text")
            .ContinueWith(async _ =>
            {
                var all = await db.Table<TrackedTimeDb>().ToListAsync();
                foreach (var trackedTimeDb in all)
                {
                    trackedTimeDb.Uuid = Guid.NewGuid();
                    await db.UpdateAsync(trackedTimeDb);
                }
            });
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
