using AroundMe.Data;
using Dapper;

namespace AroundMe.Repositories;

public class StatsRepository : IStatsRepository
{
    private readonly SqlConnectionFactory _factory;

    public StatsRepository(SqlConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<int> CountPlacesAsync()
    {
        using var connection = _factory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Places");
    }

    public async Task<int> CountUsersAsync()
    {
        using var connection = _factory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Users");
    }
}
