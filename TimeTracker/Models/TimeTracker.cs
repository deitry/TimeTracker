﻿using System.Diagnostics;

namespace TimeTracker;

public class TimeTracker
{
    public int Id { get; set; }

    public string Name { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }
    public TimeSpan ElapsedTime => IsRunning
        ? DateTime.Now - StartTime
        : EndTime - StartTime;

    public bool IsRunning { get; set; }

    public TimeTracker(string name)
    {
        Name = name;
        StartTime = DateTime.Now;
    }

    public TimeTracker(TrackedTimeDb tracker)
    {
        Id = tracker.Id;
        Name = tracker.Name;
        StartTime = tracker.StartTime;
        IsRunning = tracker.StatusEnum == TrackedTimeDb.TrackingStatus.Running;
        if (IsRunning == false)
            EndTime = tracker.StartTime + tracker.ElapsedTime;
    }

    public TimeTracker Start()
    {
        IsRunning = true;

        return this;
    }

    public TimeTracker Stop()
    {
        IsRunning = false;
        EndTime = DateTime.Now;

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
