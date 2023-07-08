using SQLite;

namespace TimeTracker;

/// <summary>
///
/// </summary>
public class TrackedTimeDb : ITable
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public Guid Uuid { get; set; }

    public string Name { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    [Ignore] public TimeSpan ElapsedTime => EndTime - StartTime;

    public int Status { get; set; }

    /// <summary>
    /// Last modification time - considering Start and Elapsed may ba changed later
    /// </summary>
    public DateTime Timestamp { get; set; }

    [Ignore]
    public TrackingStatus StatusEnum
    {
        get => (TrackingStatus) Status;
        set => Status = (int) value;
    }

    public enum TrackingStatus
    {
        Completed = 0,

        /// <summary>
        /// Currently running
        /// </summary>
        Running = 1,
    }
}
