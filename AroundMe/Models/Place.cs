namespace AroundMe.Models;

public class PlaceCard
{
    public int PlaceId { get; set; }
    public string PlaceName { get; set; } = "";
    public string Description { get; set; } = "";
    public string Location { get; set; } = "";
    public string? ImagePath { get; set; }
    public string CategoryName { get; set; } = "";
    public string Username { get; set; } = "";
    public int LikeCount { get; set; }
}

public class CategoryItem
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
}
