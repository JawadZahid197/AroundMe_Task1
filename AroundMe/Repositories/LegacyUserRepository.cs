using AroundMe.Data;
using Dapper;

namespace AroundMe.Repositories;

// The original D1 Places table uses integer UserId values.
// Identity login credentials stay in AspNetUsers, not in this table.
public class LegacyUserRepository : ILegacyUserRepository
{
    private readonly SqlConnectionFactory _factory;

    public LegacyUserRepository(SqlConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<int?> FindByEmailAsync(string email)
    {
        using var connection = _factory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<int?>(
            "SELECT UserId FROM Users WHERE Email = @Email", new { Email = email });
    }

    public async Task<int> CreateAsync(string fullName, string username, string email)
    {
        const string sql = @"INSERT INTO Users (FullName, Username, Email, PasswordHash)
            OUTPUT INSERTED.UserId
            VALUES (@FullName, @Username, @Email, @PasswordHash)";

        using var connection = _factory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, new
        {
            FullName = fullName,
            Username = username,
            Email = email,
            PasswordHash = "IDENTITY_MANAGED"
        });
    }
}
