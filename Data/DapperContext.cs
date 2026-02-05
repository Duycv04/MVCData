using Npgsql;
using System.Data;

public class DapperContext
{
    private readonly IConfiguration _config;

    public DapperContext(IConfiguration config)
    {
        _config = config;
    }

    public IDbConnection CreateConnection()
        => new NpgsqlConnection(
            _config.GetConnectionString("PostgresConnection"));
}
