using System.Diagnostics;
using Microsoft.Maui.Graphics;

namespace TimeTracker.Tests;

public class CategoriesTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestIfCategoriesEquals()
    {
        var cat1 = new CategoryDb
        {
            Id = 1,
            Name = "Review",
            CategoryGroup = "Work",
            StateEnum = CategoryDb.CategoryState.Enabled,
            ColorObject = Colors.Aqua,
        };

        var cat2 = new CategoryDb
        {
            Id = 2,
            Name = "Review",
            CategoryGroup = "Work",
            StateEnum = CategoryDb.CategoryState.Enabled,
            ColorObject = Colors.Aqua,
        };

        Assert.That(cat1, Is.EqualTo(cat2));
    }

    [Test]
    public void TestIfCategoriesList()
    {
        var cat1 = new CategoryDb
        {
            Id = 1,
            Name = "Task1",
            CategoryGroup = "Work",
            StateEnum = CategoryDb.CategoryState.Enabled,
            ColorObject = Colors.Aqua,
        };

        var cat2 = new CategoryDb
        {
            Id = 2,
            Name = "Task2",
            CategoryGroup = "Work",
            StateEnum = CategoryDb.CategoryState.Enabled,
            ColorObject = Colors.Beige,
        };

        var cat3 = new CategoryDb
        {
            Id = 3,
            Name = "Task3",
            CategoryGroup = "Personal",
            StateEnum = CategoryDb.CategoryState.Enabled,
            ColorObject = Colors.Chocolate,
        };

        var list1 = new List<CategoryDb> { cat1, cat2 };
        var list2 = new List<CategoryDb> { cat2, cat3 };

        var intersection = list1.Intersect(list2).ToList();

        Assert.That(intersection, Has.Count.EqualTo(1));
        CollectionAssert.Contains(intersection, cat2);
    }
}
