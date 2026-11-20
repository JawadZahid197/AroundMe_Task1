using AroundMe.Repositories;
using AroundMe.ViewModels;

namespace AroundMe.Services;

public class HomeService
{
    private readonly IPlaceRepository _places;
    private readonly ICategoryRepository _categories;
    private readonly IStatsRepository _stats;

    public HomeService(IPlaceRepository places, ICategoryRepository categories, IStatsRepository stats)
    {
        _places = places;
        _categories = categories;
        _stats = stats;
    }

    public async Task<HomeViewModel> GetHomeAsync(string? search, int categoryId)
    {
        return new HomeViewModel
        {
            Search = search,
            CategoryId = categoryId,
            Places = await _places.SearchAsync(search, categoryId),
            Categories = await _categories.GetAllAsync(),
            TotalPlaces = await _stats.CountPlacesAsync(),
            TotalUsers = await _stats.CountUsersAsync()
        };
    }
}
