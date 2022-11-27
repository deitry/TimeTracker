using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TimeTracker;

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
        DayStats = await DayEntries(db, DateTime.Today);
        YesterdayStats = await DayEntries(db, DateTime.Today - TimeSpan.FromDays(1));

        OnPropertyChanged(nameof(DayStats));
        OnPropertyChanged(nameof(YesterdayStats));
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

    public event PropertyChangedEventHandler? PropertyChanged;

    public List<string> YesterdayStats { get; private set; } = new List<string>();
    public List<string> DayStats { get; private set; } = new List<string>();

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
