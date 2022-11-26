using SQLite;

namespace TimeTracker;

public interface IDbMigration
{
    /// <summary>
    /// Apply migration
    /// </summary>
    Task Do(SQLiteAsyncConnection db);

    /// <summary>
    /// Revert DB scheme before migration
    /// </summary>
    Task UnDo(SQLiteAsyncConnection db);

    /// <summary>
    /// Get string representation of given migration that can be saved in DB
    /// </summary>
    string Serialize();
}
