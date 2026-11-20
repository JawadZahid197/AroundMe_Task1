using AroundMe.Models;

namespace AroundMe.Repositories;

public interface ICategoryRepository
{
    Task<List<CategoryItem>> GetAllAsync();
}
