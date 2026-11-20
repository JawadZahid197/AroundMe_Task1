using AroundMe.Data;
using AroundMe.Models;
using AroundMe.ViewModels;
using Dapper;

namespace AroundMe.Repositories;

public class PlaceRepository : IPlaceRepository
{
    private readonly SqlConnectionFactory _factory;

    public PlaceRepository(SqlConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<List<PlaceCard>> SearchAsync(string? search, int categoryId)
    {
        const string sql = @"SELECT p.PlaceId, p.PlaceName, p.Description, p.Location,
                p.ImagePath, c.CategoryName, u.Username,
                (SELECT COUNT(*) FROM Likes l WHERE l.PlaceId = p.PlaceId) AS LikeCount
            FROM Places p
            INNER JOIN Categories c ON p.CategoryId = c.CategoryId
            INNER JOIN Users u ON p.UserId = u.UserId
            WHERE (@Search = '' OR p.PlaceName LIKE '%' + @Search + '%'
                OR p.Description LIKE '%' + @Search + '%'
                OR p.Location LIKE '%' + @Search + '%')
              AND (@CategoryId = 0 OR p.CategoryId = @CategoryId)
            ORDER BY p.CreatedAt DESC";

        using var connection = _factory.CreateConnection();
        var places = await connection.QueryAsync<PlaceCard>(sql, new
        {
            Search = (search ?? "").Trim(),
            CategoryId = categoryId
        });

        return places.ToList();
    }

    public async Task<int> AddAsync(int userId, PlaceCreateViewModel model, string? imagePath)
    {
        const string sql = @"INSERT INTO Places
            (UserId, CategoryId, PlaceName, Description, Location, ImagePath)
            OUTPUT INSERTED.PlaceId
            VALUES (@UserId, @CategoryId, @PlaceName, @Description, @Location, @ImagePath)";

        using var connection = _factory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, new
        {
            UserId = userId,
            CategoryId = model.CategoryId,
            PlaceName = model.PlaceName.Trim(),
            Description = model.Description.Trim(),
            Location = model.Location.Trim(),
            ImagePath = imagePath
        });
    }
}
