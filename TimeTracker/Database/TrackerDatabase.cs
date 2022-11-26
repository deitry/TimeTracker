﻿using SQLite;

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
                Name = "Game",
                ColorObject = Colors.DarkOrchid,
                CategoryGroup = Work,
            },
            new CategoryDb()
            {
                Name = "Eat",
                ColorObject = Colors.DarkViolet,
                CategoryGroup = Work,
            },
            new CategoryDb()
            {
                Name = "Leisure",
                ColorObject = Colors.DarkGoldenrod,
                CategoryGroup = Work,
            },
            new CategoryDb()
            {
                Name = "Meditation",
                ColorObject = Colors.DarkRed,
                CategoryGroup = Work,
            },
            new CategoryDb()
            {
                Name = "Family",
                ColorObject = Colors.DarkOliveGreen,
                CategoryGroup = Work,
            },
            new CategoryDb()
            {
                Name = "Review",
                ColorObject = Colors.DarkSeaGreen,
                CategoryGroup = Work,
            },
        });

        // return _database.Table<CategoryDb>()
        //     .ToListAsync();
    }
}
