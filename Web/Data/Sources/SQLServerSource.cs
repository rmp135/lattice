using Lattice.Sources;
using Microsoft.Data.SqlClient;

namespace Lattice.Web.Data.Sources;

public class SQLServerSource : SQLSource, ISource
{
    private string connectionString;

    public SQLServerSource(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public async Task<IEnumerable<IDictionary<string, object>>> GetValues(string key)
    {
        await using var conn = new SqlConnection(connectionString);
        return await GetValues(key, conn);
    }
}
