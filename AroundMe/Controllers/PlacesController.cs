using AroundMe.Models;
using AroundMe.Services;
using AroundMe.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AroundMe.Controllers;
[Authorize] public class PlacesController(PlaceRepository places,UserManager<ApplicationUser> users,IWebHostEnvironment env):Controller
{
    [HttpGet] public IActionResult Create()
    {
        var m=new PlaceCreateViewModel();
        Fill(m);
        return View(m);
    }
    [HttpPost,ValidateAntiForgeryToken,RequestSizeLimit(6*1024*1024)] public async Task<IActionResult> Create(PlaceCreateViewModel model)
    {
        Fill(model);
        var user=await users.GetUserAsync(User);
        if(user?.LegacyUserId==null)return Challenge();
        if(model.Photo is {Length:>0})
        {
            var ext=Path.GetExtension(model.Photo.FileName).ToLowerInvariant();
            if(!new[]{".jpg",".jpeg",".png",".webp"}.Contains(ext)||model.Photo.Length>5*1024*1024)ModelState.AddModelError(nameof(model.Photo),"Use JPG, PNG or WEBP up to 5 MB.");
        }
        if(!ModelState.IsValid)return View(model);
        string? imagePath=null;
        if(model.Photo is {Length:>0})
        {
            var ext=Path.GetExtension(model.Photo.FileName).ToLowerInvariant();
            var folder=Path.Combine(env.WebRootPath,"Uploads");
            Directory.CreateDirectory(folder);
            var file=Guid.NewGuid().ToString("N")+ext;
            await using var stream=System.IO.File.Create(Path.Combine(folder,file));
            await model.Photo.CopyToAsync(stream);
            imagePath="Uploads/"+file;
        }
        places.Add(user.LegacyUserId.Value,model,imagePath);
        return RedirectToAction("Index","Home");
    }
    private void Fill(PlaceCreateViewModel model)=>model.Categories=places.Categories().Select(c=>new SelectListItem(c.CategoryName,c.CategoryId.ToString())).ToList();
}
