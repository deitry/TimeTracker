using SQLite;

namespace TimeTracker.Migrations;

public class M1_InitializeDb : IDbMigration
{
    public async Task Do(SQLiteAsyncConnection db)
    {
        await db.ExecuteAsync(@"create table if not exists main.TrackedTimeDb
        (
            Id          integer not null
        primary key autoincrement,
            Name        varchar,
        StartTime   bigint,
            ElapsedTime bigint
            );
        ");

        await db.ExecuteAsync(@"
CREATE TABLE if not exists ControlDb (
    ""Id"" integer primary key not null ,
    ""Value"" varchar )
");

        await db.InsertOrReplaceAsync(new ControlDb(ControlDb.ParamId.Version, 1));
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
