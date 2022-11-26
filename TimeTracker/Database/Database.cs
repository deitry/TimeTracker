using SQLite;

namespace TimeTracker;

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
        await Migrator.Migrate(_database);
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
