using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TimeTracker;

public sealed class ViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public string TimeLeftString => (TimeSpan.FromMinutes(30) - _stopwatch.Elapsed).ToString(@"hh\:mm\:ss");
    // private TimeSpan _timeLeft = TimeSpan.FromMinutes(30);
    private readonly Stopwatch _stopwatch = new Stopwatch();
    private readonly Timer _timer;
    private const double Period = 1;

    public ViewModel()
    {
        _timer = new Timer(Count, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
    }

    public void Activate()
    {
        if (_stopwatch.IsRunning)
        {
            Reset();
        }
        else
        {
            Start();
        }
    }

    public void Reset()
    {
        _stopwatch.Reset();
        _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

        OnPropertyChanged(nameof(TimeLeftString));
    }

    public void Count(object obj)
    {
        OnPropertyChanged(nameof(TimeLeftString));
    }

    public void Start()
    {
        _stopwatch.Start();
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
