using AroundMe.Services;
using Microsoft.AspNetCore.Mvc;

namespace AroundMe.Controllers;

public class HomeController : Controller
{
    private readonly HomeService _home;

    public HomeController(HomeService home)
    {
        _home = home;
    }

    public async Task<IActionResult> Index(string? search, int categoryId = 0)
    {
        var model = await _home.GetHomeAsync(search, categoryId);
        return View(model);
    }

    // AJAX updates only the place cards; the full page stays open.
    [HttpGet]
    public async Task<IActionResult> SearchResults(string? search, int categoryId = 0)
    {
        var model = await _home.GetHomeAsync(search, categoryId);
        return PartialView("_PlaceResults", model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}
