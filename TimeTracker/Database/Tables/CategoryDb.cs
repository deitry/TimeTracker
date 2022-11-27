using Microsoft.Maui.Graphics.Text;
using SQLite;

namespace TimeTracker;

public class CategoryDb : ITable
{
    public enum CategoryState
    {
        Enabled = 0,
        Disabled = 1,
    }

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int State { get; set; }

    [Ignore]
    public CategoryState StateEnum
    {
        get => (CategoryState) State;
        set => State = (int) value;
    }

    public string Name { get; set; }

    /// <summary>
    /// Group of categories
    /// </summary>
    public string CategoryGroup { get; set; }

    public string ColorString { get; set; }

    [Ignore]
    public Color ColorObject
    {
        get => Color.Parse(ColorString);
        set => ColorString = value.ToArgbHex();
    }

    public override string ToString()
    {
        return $"Category: {Name}";
    }
}
