using System.Data;
using Dapper;

namespace Lattice.Web.Data.Sources;

public abstract class SQLSource
{
    public async Task<IEnumerable<IDictionary<string, object>>> GetValues(string key, IDbConnection connection)
    {
        return (
                key.StartsWith("SELECT", StringComparison.InvariantCultureIgnoreCase) ?
                await connection.QueryAsync(key) :
                await connection.QueryAsync($"SELECT * FROM {key}")
            ).Cast<IDictionary<string, object>>();
    }
}
