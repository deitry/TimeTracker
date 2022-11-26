using Microsoft.Maui.Graphics.Text;
using SQLite;

namespace TimeTracker;

public class CategoryDb : ITable
{
    public int Id { get; set; }

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
        set => ColorString = value.ToString();
    }
}
