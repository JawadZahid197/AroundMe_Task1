using AroundMe.Models;
using AroundMe.Repositories;
using AroundMe.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AroundMe.Controllers;

[Authorize]
public class PlacesController : Controller
{
    private readonly IPlaceRepository _places;
    private readonly ICategoryRepository _categories;
    private readonly UserManager<ApplicationUser> _users;
    private readonly IWebHostEnvironment _environment;

    public PlacesController(IPlaceRepository places, ICategoryRepository categories,
        UserManager<ApplicationUser> users, IWebHostEnvironment environment)
    {
        _places = places;
        _categories = categories;
        _users = users;
        _environment = environment;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new PlaceCreateViewModel();
        await FillCategoriesAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<IActionResult> Create(PlaceCreateViewModel model)
    {
        await FillCategoriesAsync(model);
        var user = await _users.GetUserAsync(User);

        if (user?.LegacyUserId == null)
        {
            return Challenge();
        }

        if (model.Photo != null && model.Photo.Length > 0)
        {
            var extension = Path.GetExtension(model.Photo.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };

            if (!allowed.Contains(extension) || model.Photo.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError(nameof(model.Photo), "Use JPG, PNG or WEBP up to 5 MB.");
            }
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string? imagePath = null;
        if (model.Photo != null && model.Photo.Length > 0)
        {
            var extension = Path.GetExtension(model.Photo.FileName).ToLowerInvariant();
            var folder = Path.Combine(_environment.WebRootPath, "Uploads");
            Directory.CreateDirectory(folder);
            var fileName = Guid.NewGuid().ToString("N") + extension;
            await using var stream = System.IO.File.Create(Path.Combine(folder, fileName));
            await model.Photo.CopyToAsync(stream);
            imagePath = "Uploads/" + fileName;
        }

        await _places.AddAsync(user.LegacyUserId.Value, model, imagePath);
        return RedirectToAction("Index", "Home");
    }

    private async Task FillCategoriesAsync(PlaceCreateViewModel model)
    {
        var categories = await _categories.GetAllAsync();
        model.Categories = categories.Select(category => new SelectListItem
        {
            Text = category.CategoryName,
            Value = category.CategoryId.ToString()
        }).ToList();
    }
}
