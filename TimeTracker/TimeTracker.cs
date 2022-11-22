using System.Diagnostics;

namespace TimeTracker;

public class TimeTracker
{
    public string Name { get; set; }

    public DateTime StartTime { get; set; }

    public TimeSpan ElapsedTime => _sw.Elapsed;

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
            Name = Name,
            StartTime = StartTime,
            ElapsedTime = ElapsedTime,
        };
    }
}
