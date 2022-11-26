using SQLite;

namespace TimeTracker;

public class TrackerDatabase
{
    private readonly SQLiteAsyncConnection _database;

    private TrackerDatabase()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var path = Path.Combine(home, Constants.DatabasePath);

        _database = new SQLiteAsyncConnection(path, Constants.Flags);
    }

    public static readonly AsyncLazy<TrackerDatabase> Instance = new(async () => await new TrackerDatabase().InitializeDb());

    public Task InsertAsync<T>(T result) where T : ITable, new()
    {
        return _database.InsertAsync(result);
    }

    private async Task<TrackerDatabase> InitializeDb()
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

    public const string Personal = nameof(Personal);
    public const string Work = nameof(Work);

    public Task<List<CategoryDb>> GetCategories()
    {
        return Task.FromResult(new List<CategoryDb>()
        {
            new CategoryDb()
            {
                Name = "Code",
                Enabled = true,
                ColorObject = Colors.DarkSlateBlue,
                CategoryGroup = Work,
            },
            new CategoryDb()
            {
                Name = "Tasks",
                Enabled = true,
                ColorObject = Colors.DarkCyan,
                CategoryGroup = Work,
            },
            new CategoryDb()
            {
                Name = "Call",
                Enabled = true,
                ColorObject = Colors.DarkGreen,
                CategoryGroup = Work,
            },
            new CategoryDb()
            {
                Name = "Review",
                Enabled = true,
                ColorObject = Colors.DarkSeaGreen,
                CategoryGroup = Work,
            },
            new CategoryDb()
            {
                Name = "Pet",
                Enabled = true,
                ColorObject = Colors.DarkKhaki,
                CategoryGroup = Work,
            },
            new CategoryDb()
            {
                Name = "Sport",
                Enabled = true,
                ColorObject = Colors.LightGray,
                CategoryGroup = Personal,
            },
            new CategoryDb()
            {
                Name = "Game",
                Enabled = true,
                ColorObject = Colors.DarkOrchid,
                CategoryGroup = Personal,
            },
            new CategoryDb()
            {
                Name = "Eat",
                Enabled = true,
                ColorObject = Colors.DarkViolet,
                CategoryGroup = Personal,
            },
            new CategoryDb()
            {
                Name = "Leisure",
                Enabled = true,
                ColorObject = Colors.DarkGoldenrod,
                CategoryGroup = Personal,
            },
            new CategoryDb()
            {
                Name = "Meditation",
                Enabled = true,
                ColorObject = Colors.DarkRed,
                CategoryGroup = Personal,
            },
            new CategoryDb()
            {
                Name = "Family",
                Enabled = true,
                ColorObject = Colors.DarkOliveGreen,
                CategoryGroup = Personal,
            },
        });

        // return _database.Table<CategoryDb>()
        //     .ToListAsync();
    }
}
