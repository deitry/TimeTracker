using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Layouts;

namespace TimeTracker;

internal sealed class ViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public const string TimeSpanHmsFormat = @"hh\:mm\:ss";

    public TimeSpan? TimeElapsed => _trackers.FirstOrDefault().Value?.ElapsedTime;
    public string TimeElapsedString => TimeElapsed?.ToString(TimeSpanHmsFormat) ?? "=";

    private readonly Dictionary<string, TimeTracker> _trackers = new();
    private readonly Timer _timer;
    private readonly TimeSpan Period = TimeSpan.FromSeconds(1);

    private readonly TimeSpan TimeLimit = TimeSpan.FromMinutes(30);

    public delegate void AlertHandler(TimeTracker timeTracker);
    public event AlertHandler? Alert;

    public ViewModel()
    {
        _timer = new Timer(Tick, null, TimeSpan.Zero, Period);

        var context = SynchronizationContext.Current;
        Debug.Assert(context != null);
    }

    public async Task InitializeRunningTrackers()
    {
        var db = await TrackerDatabase.Instance;
        var trackers = await db.ListRunningTrackers();

        foreach (var tracker in trackers)
        {
            _trackers[tracker.Name] = tracker;
        }

        Categories = await db.GetCategories();
    }

    public List<CategoryDb> Categories { get; set; }

    public void Activate(bool tbIsToggled, string tbText)
    {
        if (tbIsToggled)
        {
            Start(tbText);
        }
        else
        {
            Reset(tbText);
        }
    }

    public void Reset(string tbText)
    {
        // _timerStopwatch.Reset();

        if (_trackers.ContainsKey(tbText))
        {
            var timeTracker = _trackers[tbText].Stop();
            _trackers.Remove(tbText);

            this.Alert?.Invoke(timeTracker);
        }

        // if (!_trackers.Any())
        //     _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

        OnPropertyChanged(nameof(TimeElapsedString));
    }

    public void Tick(object? obj)
    {
        // save to DB
        Task.Run(async () =>
        {
            var db = await TrackerDatabase.Instance;
            foreach (var tracker in _trackers.Values)
            {
                await db.Update(tracker);
            }
        });

        OnPropertyChanged(nameof(TimeElapsedString));
    }

    public void Start(string tbText)
    {
        if (_trackers.ContainsKey(tbText))
            return;

        _trackers[tbText] = new TimeTracker(tbText).Start();

        // _timer.Change(TimeSpan.Zero, Period);
    }

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

    public bool GetTrackerState(CategoryDb category)
    {
        return _trackers.ContainsKey(category.Name);
    }
}
