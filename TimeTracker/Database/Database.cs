using SQLite;

namespace TimeTracker;

public static class Constants
{
    public const string DatabasePath = "tracker.db";

    public const SQLiteOpenFlags Flags =
        // open the database in read/write mode
        SQLiteOpenFlags.ReadWrite |
        // create the database if it doesn't exist
        SQLiteOpenFlags.Create |
        // enable multi-threaded database access
        SQLiteOpenFlags.SharedCache;
}

public interface ITable
{
    int Id { get; }
}

public class TrackedTimeDb : ITable
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }

    public DateTime StartTime { get; set; }

    public TimeSpan ElapsedTime { get; init; }
}

public class Database
{
    private readonly SQLiteAsyncConnection _database;

    private Database()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var path = Path.Combine(home, Constants.DatabasePath);

        _database = new SQLiteAsyncConnection(path, Constants.Flags);
    }

    public static readonly AsyncLazy<Database> Instance = new(async () => await new Database().InitializeDb());

    public Task InsertAsync<T>(T result) where T : ITable, new()
    {
        return _database.InsertAsync(result);
    }

    private async Task<Database> InitializeDb()
    {
        await _database.CreateTableAsync<TrackedTimeDb>();

        return this;
    }

    public Task<List<TrackedTimeDb>> ListDay(DateTime day)
    {
        var fullDay = day + TimeSpan.FromDays(1);

        return _database.Table<TrackedTimeDb>()
            .Where(t => t.StartTime >= day && t.StartTime < fullDay)
            .ToListAsync();
    }
}
