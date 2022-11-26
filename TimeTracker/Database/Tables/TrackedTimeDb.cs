using SQLite;

namespace TimeTracker;

public class TrackedTimeDb : ITable
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }

    public DateTime StartTime { get; set; }

    public TimeSpan ElapsedTime { get; init; }
}
