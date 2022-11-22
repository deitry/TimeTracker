using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TimeTracker;

internal sealed class ViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public string TimeLeftString
    {
        get
        {
            var first = _trackers.FirstOrDefault().Value;
            if (first != null)
            {
                return first.ElapsedTime.ToString(@"hh\:mm\:ss");
            }

            return string.Empty;
        }
    }

    private readonly Dictionary<string, TimeTracker> _trackers = new();
    private readonly Timer _timer;
    private const double Period = 1;

    private readonly TimeSpan TimeLimit = TimeSpan.FromMinutes(30);

    public delegate void AlertHandler(TimeTracker timeTracker);
    public event AlertHandler Alert;

    public ViewModel()
    {
        _timer = new Timer(Tick, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
    }

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

        if (!_trackers.Any())
            _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

        OnPropertyChanged(nameof(TimeLeftString));
    }

    public void Tick(object? obj)
    {
        OnPropertyChanged(nameof(TimeLeftString));
    }

    public void Start(string tbText)
    {
        if (_trackers.ContainsKey(tbText))
            return;

        _trackers[tbText] = new TimeTracker(tbText).Start();

        _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(Period));
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
