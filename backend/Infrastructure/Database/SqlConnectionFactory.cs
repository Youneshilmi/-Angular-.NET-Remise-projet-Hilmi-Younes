using Microsoft.Data.SqlClient;

namespace Lensrock.Infrastructure.Database;

public sealed class SqlConnectionFactory(string connectionString)
{
    public SqlConnection Create() => new(connectionString);
}
