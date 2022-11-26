using SQLite;

namespace TimeTracker.Database.Migrations;

public class M2_AddFields : IDbMigration
{
    public Task Do(SQLiteAsyncConnection db)
    {
        return db.ExecuteAsync(@"create table main.CategoryDb
        (
            Id          integer not null
        primary key autoincrement,
            Name        varchar,
            Group       varchar,
            Color       varchar
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
