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
        new M2_AddCategories(),
        new M3_AddStatuses(),
        new M4_AddUuid(),
        new M005_AddTimestampAndReworkElapsed(),
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

            try
            {
                await migration.Do(db);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Migration {migration.GetType().Name} failed: {e}");
                throw;
            }
            await db.UpdateAsync(new ControlDb(ControlDb.ParamId.Version, i + 1));
        }
    }
}
