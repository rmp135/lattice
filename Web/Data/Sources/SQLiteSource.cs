using Lattice.Sources;
using Microsoft.Data.Sqlite;

namespace Lattice.Web.Data.Sources;

public class SQLiteSource : SQLSource, ISource
{
    private readonly string connectionString;

    public SQLiteSource(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public async Task<IEnumerable<IDictionary<string, object>>> GetValues(string key)
    {
        await using var conn = new SqliteConnection(connectionString);
        return await GetValues(key, conn);
    }
}
