using SQLite;

namespace TimeTracker.Database.Migrations;

public class M2_AddCategories : IDbMigration
{
    public async Task Do(SQLiteAsyncConnection db)
    {
        await db.ExecuteAsync(@"create table if not exists main.CategoryDb
        (
            Id            integer not null
        primary key autoincrement,
            State         integer,
            Name          varchar,
            CategoryGroup varchar,
            ColorString   varchar
            );
        ");

        var categories = await TrackerDatabase.DefaultCategories();
        db.InsertAllAsync(categories);
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
