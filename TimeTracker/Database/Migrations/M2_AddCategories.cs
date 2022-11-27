using SQLite;

namespace TimeTracker.Database.Migrations;

public class M2_AddCategories : IDbMigration
{
    public Task Do(SQLiteAsyncConnection db)
    {
        return db.ExecuteAsync(@"create table if not exists main.CategoryDb
        (
            Id            integer not null
        primary key autoincrement,
            State         integer,
            Name          varchar,
            CategoryGroup varchar,
            ColorString   varchar
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
