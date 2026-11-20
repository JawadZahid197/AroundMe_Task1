using Microsoft.Data.SqlClient;

namespace AroundMe.Data;

public class SqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("AroundMeDB")
            ?? throw new InvalidOperationException("AroundMeDB connection string is missing.");
    }

    public SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
