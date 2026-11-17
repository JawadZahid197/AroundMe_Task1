using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AroundMe.ViewModels;
public class PlaceCreateViewModel
{
    [Required, StringLength(150)] public string PlaceName
    {
        get;
        set;
    }
    ="";
    [Required] public int? CategoryId
    {
        get;
        set;
    }
    [Required, StringLength(250)] public string Location
    {
        get;
        set;
    }
    ="";
    [Required, StringLength(1000)] public string Description
    {
        get;
        set;
    }
    ="";
    public IFormFile? Photo
    {
        get;
        set;
    }
    public List<SelectListItem> Categories
    {
        get;
        set;
    }
    =new();
}
