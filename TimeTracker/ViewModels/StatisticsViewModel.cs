using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TimeTracker;

public static class DateTimeExtensions
{
    public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
    {
        int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }
}
public sealed class StatisticsViewModel : INotifyPropertyChanged
{
    private readonly SynchronizationContext? _context;

    public StatisticsViewModel()
    {
        _context = SynchronizationContext.Current;

        Task.Run(async () => await Refresh());
    }

    async Task Refresh()
    {
        var db = await TrackerDatabase.Instance;
        var today = DateTime.Today;
        var yesterday = today - TimeSpan.FromDays(1);
        DayStats = await DayEntries(db, today);
        YesterdayStats = await DayEntries(db, yesterday);

        DayGroupStats = await DayGroupEntries(db, today);
        YesterdayGroupStats = await DayGroupEntries(db, yesterday);

        WeekWorkStats = await WeekWorkEntries(db);

        OnPropertyChanged(nameof(DayStats));
        OnPropertyChanged(nameof(DayGroupStats));

        OnPropertyChanged(nameof(YesterdayStats));
        OnPropertyChanged(nameof(YesterdayGroupStats));

        OnPropertyChanged(nameof(WeekWorkStats));
    }

    private async Task<List<string>> DayEntries(TrackerDatabase db, DateTime day)
    {
        var list = await db.ListDay(day);

        if (_context != null)
            await _context;

        var totalTime = new Dictionary<string, TimeSpan>();

        foreach (var entry in list)
        {
            if (!totalTime.ContainsKey(entry.Name))
                totalTime.Add(entry.Name, default);

            totalTime[entry.Name] += entry.ElapsedTime;
        }

        var kvps = totalTime.ToList();
        kvps.Sort((first, second) => second.Value.CompareTo(first.Value));

        return kvps.Select(t => $"{t.Key} : {t.Value.ToString(ViewModel.TimeSpanHmsFormat)}").ToList();
    }

    // TODO: combine with DayEntries
    private async Task<List<string>> DayGroupEntries(TrackerDatabase db, DateTime day)
    {
        var enumerator = (await db.GetCategories()).Select(c => KeyValuePair.Create(c.Name, c.CategoryGroup));
        var categoriesToGroupMap = new Dictionary<string, string>(enumerator);

        var list = await db.ListDay(day);

        if (_context != null)
            await _context;

        var totalTime = new Dictionary<string, TimeSpan>();

        foreach (var entry in list)
        {
            categoriesToGroupMap.TryGetValue(entry.Name, out var group);
            if (string.IsNullOrEmpty(group))
                group = entry.Name;

            if (!totalTime.ContainsKey(group))
                totalTime.Add(group, default);

            totalTime[group] += entry.ElapsedTime;
        }

        var kvps = totalTime.ToList();
        kvps.Sort((first, second) => second.Value.CompareTo(first.Value));

        return kvps.Select(t => $"{t.Key} : {t.Value.Hours} hr {t.Value.Minutes} min").ToList();
    }

    // TODO: combine with DayEntries
    private async Task<List<string>> WeekWorkEntries(TrackerDatabase db)
    {
        var enumerator = (await db.GetCategories()).Select(c => KeyValuePair.Create(c.Name, c.CategoryGroup));
        var categoriesToGroupMap = new Dictionary<string, string>(enumerator);

        var now = DateTime.Now;
        var list = await db.ListDateTimeRange(now.StartOfWeek(DayOfWeek.Monday), now);

        if (_context != null)
            await _context;

        var totalTime = new Dictionary<string, TimeSpan>();

        foreach (var entry in list)
        {
            categoriesToGroupMap.TryGetValue(entry.Name, out var group);
            if (string.IsNullOrEmpty(group))
                group = entry.Name;

            if (!totalTime.ContainsKey(group))
                totalTime.Add(group, default);

            totalTime[group] += entry.ElapsedTime;
        }

        var kvps = totalTime.ToList();
        kvps.Sort((first, second) => second.Value.CompareTo(first.Value));

        return kvps.Select(t => $"{t.Key} : {(int) t.Value.TotalHours} hr {t.Value.Minutes} min").ToList();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public List<string> YesterdayStats { get; private set; } = new List<string>();
    public List<string> DayStats { get; private set; } = new List<string>();

    public List<string> YesterdayGroupStats { get; private set; } = new List<string>();
    public List<string> DayGroupStats { get; private set; } = new List<string>();
    public List<string> WeekWorkStats { get; private set; } = new List<string>();

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
