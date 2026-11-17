using AroundMe.Services;
using Microsoft.AspNetCore.Mvc;

namespace AroundMe.Controllers;
public class HomeController(PlaceRepository places):Controller
{
    public IActionResult Index(string? search,int categoryId=0)=>View(places.Home(search,categoryId));
    public IActionResult Error()=>View();
}
