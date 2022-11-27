using System.Diagnostics;
using SQLite;
using TimeTracker.Database.Migrations;
using TimeTracker.Migrations;

namespace TimeTracker;


public static class Migrator
{
    /// <summary>
    /// Order should not be ever changed.
    /// Name of migration should include its index for transparency.
    /// </summary>
    private static readonly List<IDbMigration> Migrations = new()
    {
        new M1_InitializeDb(),
        new M2_AddFields(),
    };

    public static async Task Migrate(SQLiteAsyncConnection db)
    {
        int version = 0;
        try
        {
            var versionParam = await db.FindAsync<ControlDb>(pk: (int)ControlDb.ParamId.Version);
            version = versionParam?.AsInt ?? 0;
        }
        catch (SQLiteException)
        {
            // assuming no ControlDb yet in DB, same as version == 0
        }

        for (var i = version; i < Migrations.Count; i++)
        {
            var migration = Migrations[i];

            await migration.Do(db);
            await db.UpdateAsync(new ControlDb(ControlDb.ParamId.Version, i + 1));
        }
    }
}
