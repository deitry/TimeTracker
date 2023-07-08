using System.Text.Json.Serialization;
using Microsoft.Maui.Graphics;
using SQLite;

namespace TimeTracker;

public class CategoryDb : ITable
{
    public enum CategoryState
    {
        Enabled = 0,
        Disabled = 1,
    }

    [PrimaryKey, AutoIncrement, JsonIgnore] public int Id { get; set; }

    public int State { get; set; }

    [Ignore, JsonIgnore]
    public CategoryState StateEnum
    {
        get => (CategoryState)State;
        set => State = (int)value;
    }

    public string Name { get; set; }

    /// <summary>
    /// Group of categories
    /// </summary>
    public string CategoryGroup { get; set; }

    public string ColorString { get; set; }

    [Ignore, JsonIgnore]
    public Color ColorObject
    {
        get => Color.Parse(ColorString);
        init => ColorString = value.ToArgbHex();
    }

    public override string ToString()
    {
        return $"Category: {Name}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not CategoryDb other)
            return false;

        return Equals(other);
    }

    protected bool Equals(CategoryDb other)
    {
        // time trackers reference only category name, so we can compare only by name
        return Name == other.Name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, CategoryGroup, State, ColorString);
    }
}
