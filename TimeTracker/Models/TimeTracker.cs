using System.Diagnostics;

namespace TimeTracker;

public class TimeTracker
{
    public int Id { get; set; }

    public string Name { get; set; }

    public DateTime StartTime { get; set; }

    public TimeSpan ElapsedTime => _sw.Elapsed;
    public bool IsRunning => _sw.IsRunning;

    private readonly Stopwatch _sw = new Stopwatch();

    public TimeTracker(string name)
    {
        Name = name;
        StartTime = DateTime.Now;
    }

    public TimeTracker Start()
    {
        _sw.Start();

        return this;
    }

    public TimeTracker Stop()
    {
        _sw.Stop();

        return this;
    }

    public TrackedTimeDb ToDb()
    {
        return new TrackedTimeDb
        {
            Id = Id,
            Name = Name,
            StartTime = StartTime,
            ElapsedTime = ElapsedTime,
            StatusEnum = IsRunning
                ? TrackedTimeDb.TrackingStatus.Running
                : TrackedTimeDb.TrackingStatus.Completed,
        };
    }
}
