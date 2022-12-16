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
        _timer = new Timer(Tick, null, Timeout.Infinite, Timeout.Infinite);
    }

    public async Task InitializeRunningTrackers()
    {
        var db = await TrackerDatabase.Instance;
        var trackers = await db.ListRunningTrackers();

        // var groups = trackers.GroupBy(t => t.StartTime);
        //
        // foreach (var group in groups)
        // {
        //     var grouped = group.ToList();
        //     var sorted = grouped.OrderByDescending(t => t.ElapsedTime);
        //
        //     foreach (var tracker in sorted.Skip(1))
        //     {
        //         await db.Remove(tracker);
        //     }
        // }

        foreach (var tracker in trackers)
        {
            if (_trackers.ContainsKey(tracker.Name))
            {
                // force stop tracker
                tracker.StatusEnum = TrackedTimeDb.TrackingStatus.Completed;
                await db.Update(tracker);
            }
            else
            {
                _trackers[tracker.Name] = new TimeTracker(tracker);
            }
        }

        Categories = await db.GetCategories();
    }

    public void StartTimer()
    {
        _timer.Change(TimeSpan.Zero, Period);
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

    private readonly object _ticker = new();
    private bool _ticking = false;

    private void Tick(object? obj)
    {
        lock (_ticker)
        {
            // if we triggered before previous Tick ends, most likely we are in debug mode
            if (_ticking)
                return;

            _ticking = true;
        }

        // save to DB
        Task.Run(async () =>
        {
            var db = await TrackerDatabase.Instance;
            foreach (var entry in _trackers.Keys)
            {
                var tracker = _trackers[entry];
                if (tracker.StartTime.Date < DateTime.Today)
                {
                    tracker.Stop();

                    // TODO: track multiple days as separate entries
                    tracker.EndTime = DateTime.Today;
                    await db.Update(tracker);

                    _trackers[entry] = new TimeTracker(entry)
                    {
                        StartTime = DateTime.Today,
                    }.Start();
                }

                await db.Update(_trackers[entry]);
            }

            lock (_ticker)
            {
                _ticking = false;
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
