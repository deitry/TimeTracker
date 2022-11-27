using SQLite;

namespace TimeTracker;

public class TrackedTimeDb : ITable
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }

    public DateTime StartTime { get; set; }

    public TimeSpan ElapsedTime { get; init; }

    public int Status { get; set; }

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
        Started = 1,
    }
}
