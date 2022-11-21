using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TimeTracker;

public sealed class AlertViewModel : INotifyPropertyChanged
{
    private TimeTracker? _tracker;

    public string TrackerName => _tracker?.Name ?? string.Empty;
    public string TrackerMessage => $"Elapsed:\n{_tracker?.ElapsedTime:g}";

    public event PropertyChangedEventHandler? PropertyChanged;

    public void SetTracker(TimeTracker tracker)
    {
        _tracker = tracker;

        OnPropertyChanged(nameof(TrackerName));
        OnPropertyChanged(nameof(TrackerMessage));
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
