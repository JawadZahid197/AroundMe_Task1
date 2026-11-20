using AroundMe.Models;

namespace AroundMe.ViewModels;

public class HomeViewModel
{
    public string? Search { get; set; }
    public int CategoryId { get; set; }
    public List<PlaceCard> Places { get; set; } = new();
    public List<CategoryItem> Categories { get; set; } = new();
    public int TotalPlaces { get; set; }
    public int TotalUsers { get; set; }
}
