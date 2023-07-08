using SQLite;

namespace TimeTracker.Database.Migrations;

public class M3_AddStatuses : IDbMigration
{
    public Task Do(SQLiteAsyncConnection db)
    {
        return db.ExecuteAsync("ALTER TABLE TrackedTimeDb ADD Status integer DEFAULT 0");
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
