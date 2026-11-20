using AroundMe.Data;
using AroundMe.Models;
using Dapper;

namespace AroundMe.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly SqlConnectionFactory _factory;

    public CategoryRepository(SqlConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<List<CategoryItem>> GetAllAsync()
    {
        const string sql = "SELECT CategoryId, CategoryName FROM Categories ORDER BY CategoryName";
        using var connection = _factory.CreateConnection();
        var categories = await connection.QueryAsync<CategoryItem>(sql);
        return categories.ToList();
    }
}
