using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace TimeTracker.Server.Controllers;

[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ILogger<CategoriesController> logger)
    {
        _logger = logger;
    }

    [HttpGet(template: "get", Name = "GetAllCategories")]
    public async Task<IEnumerable<CategoryDb>> Get()
    {
        var db = await TrackerDatabase.Instance;
        var categories = await db.GetCategories();

        return categories;
    }

    [HttpPost(template: "update", Name = "UpdateCategories")]
    public async Task<IActionResult> UpdateCategories([FromBody] List<CategoryDb> newCategories)
    {
        var db = await TrackerDatabase.Instance;
        var oldCategories = await db.GetCategories();

        var added = newCategories.Except(oldCategories);
        var removed = oldCategories.Except(newCategories);
        var modified = newCategories.Intersect(oldCategories);

        foreach (var category in added)
        {
            await db.InsertAsync(category);
        }

        foreach (var category in removed)
        {
            await db.RemoveAsync(category);
        }

        foreach (var category in modified)
        {
            await db.Update(category);
        }

        return Ok();
    }
}
