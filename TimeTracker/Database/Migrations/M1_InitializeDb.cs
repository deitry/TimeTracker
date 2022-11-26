using SQLite;

namespace TimeTracker.Migrations;

public class M1_InitializeDb : IDbMigration
{
    public Task Do(SQLiteAsyncConnection db)
    {
        return db.ExecuteAsync(@"create table main.TrackedTimeDb
        (
            Id          integer not null
        primary key autoincrement,
            Name        varchar,
        StartTime   bigint,
            ElapsedTime bigint
            );
        ");
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
