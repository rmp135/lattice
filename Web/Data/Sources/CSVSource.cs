using System.Globalization;
using CsvHelper;
using Lattice.Sources;

namespace Lattice.Web.Data.Sources;

public class CSVSource : ISource
{
    private readonly IEnumerable<IDictionary<string, object>> data;

    private readonly string sourceName; 
    
    public CSVSource(string filename)
    {
        sourceName = Path.GetFileNameWithoutExtension(filename);
        using var reader = new StreamReader(filename);
        using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
        // Read entire file and convert to dictionary.
        data = csvReader.GetRecords<dynamic>().Select(a => a as IDictionary<string, object>).ToArray();
    }

    public Task<IEnumerable<IDictionary<string, object>>> GetValues(string key)
    {
        return Task.FromResult(
                !string.Equals(key, sourceName, StringComparison.InvariantCultureIgnoreCase) ?
                Enumerable.Empty<IDictionary<string, object>>() :
                data
            );
    }
}
