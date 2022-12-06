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
        return _database.Table<CategoryDb>()
            .Where(c => c.State == (int)CategoryDb.CategoryState.Enabled)
            .ToListAsync();
    }

    private static Task<List<CategoryDb>> DefaultCategories()
    {
        return Task.FromResult(new List<CategoryDb>()
        {
            new CategoryDb()
            {
                Name = "Code",
                ColorObject = Colors.DarkSlateBlue,
                CategoryGroup = Work,
            },
            new CategoryDb()
            {
                Name = "Tasks",
                ColorObject = Colors.DarkCyan,
                CategoryGroup = Work,
            },
            new CategoryDb()
            {
                Name = "Call",
                ColorObject = Colors.DarkGreen,
                CategoryGroup = Work,
            },
            new CategoryDb()
            {
                Name = "Review",
                ColorObject = Colors.DarkSeaGreen,
                CategoryGroup = Work,
            },
            new CategoryDb()
            {
                Name = "Pet",
                ColorObject = Colors.DarkKhaki,
                CategoryGroup = Work,
            },
            new CategoryDb()
            {
                Name = "Sport",
                ColorObject = Colors.LightGray,
                CategoryGroup = Personal,
            },
            new CategoryDb()
            {
                Name = "Game",
                ColorObject = Colors.DarkOrchid,
                CategoryGroup = Personal,
            },
            new CategoryDb()
            {
                Name = "Eat",
                ColorObject = Colors.DarkViolet,
                CategoryGroup = Personal,
            },
            new CategoryDb()
            {
                Name = "Leisure",
                ColorObject = Colors.DarkGoldenrod,
                CategoryGroup = Personal,
            },
            new CategoryDb()
            {
                Name = "Meditation",
                ColorObject = Colors.DarkRed,
                CategoryGroup = Personal,
            },
            new CategoryDb()
            {
                Name = "Family",
                ColorObject = Colors.DarkOliveGreen,
                CategoryGroup = Personal,
            },
        });
    }

    private readonly SemaphoreSlim _dbSemaphore = new SemaphoreSlim(1, 1);

    public async Task Update(TimeTracker tracker)
    {
        var serialized = tracker.ToDb();

        await Update(serialized);

        tracker.Id = serialized.Id;
    }

    public async Task Update(TrackedTimeDb tracker)
    {
        await _dbSemaphore.WaitAsync();

        try
        {
            if (tracker.Id == 0)
            {
                await _database.InsertAsync(tracker);
                tracker.Id = tracker.Id;
            }
            else
            {
                await _database.UpdateAsync(tracker);
            }
        }
        finally
        {
            _dbSemaphore.Release();
        }
    }

    public async Task<List<TrackedTimeDb>> ListRunningTrackers()
    {
        await _dbSemaphore.WaitAsync();

        try
        {
            var trackers = await _database.Table<TrackedTimeDb>()
                .Where(t => t.Status == (int) TrackedTimeDb.TrackingStatus.Running)
                .OrderByDescending(t => t.StartTime)
                // .OrderByDescending(t => t.ElapsedTime)
                .ToListAsync();

            return trackers;
        }
        finally
        {
            _dbSemaphore.Release();
        }
    }

    public async Task Remove(TrackedTimeDb tracker)
    {
        await _dbSemaphore.WaitAsync();

        try
        {
            await _database.DeleteAsync(tracker);
        }
        finally
        {
            _dbSemaphore.Release();
        }
    }
}
