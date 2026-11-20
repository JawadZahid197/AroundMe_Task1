using AroundMe.Models;
using AroundMe.ViewModels;

namespace AroundMe.Repositories;

public interface IPlaceRepository
{
    Task<List<PlaceCard>> SearchAsync(string? search, int categoryId);
    Task<int> AddAsync(int userId, PlaceCreateViewModel model, string? imagePath);
}
