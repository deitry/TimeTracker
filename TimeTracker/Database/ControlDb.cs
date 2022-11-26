using SQLite;

namespace TimeTracker;

public class ControlDb : ITable
{
    public enum ParamId
    {
        Version,
    }

    public int Id { get; set; }

    [Ignore]
    public ParamId Param
    {
        get => (ParamId) Id;
        set => Id = (int) value;
    }

    [Ignore]
    public int? AsInt
    {
        get
        {
            if (int.TryParse(Value, out var intValue))
                return intValue;

            return null;
        }
        set => Value = value.ToString() ?? string.Empty;
    }

    public string Value { get; private set; } = string.Empty;

    public ControlDb() { }

    public ControlDb(ParamId id, int value)
    {
        Param = id;
        Value = value.ToString();
    }

    public ControlDb(ParamId id, string value)
    {
        Param = id;
        Value = value;
    }
}
