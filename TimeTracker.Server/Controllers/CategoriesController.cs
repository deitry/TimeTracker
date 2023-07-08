using Microsoft.AspNetCore.Mvc;

namespace TimeTracker.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ILogger<CategoriesController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<CategoryDb>> Get()
    {
        var db = await TrackerDatabase.Instance;
        var categories = await db.GetCategories();

        return categories.ToArray();
    }
}
