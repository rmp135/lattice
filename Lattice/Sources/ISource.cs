namespace Lattice.Sources;

/// <summary>
/// An interface for retrieving from an external data source.
/// </summary>
public interface ISource
{
    /// <summary>
    /// Retrieve values from an external data source.
    /// </summary>
    /// <param name="key">The full token that is passed in from the template.</param>
    /// <returns>Key/value pair of records.</returns>
    public Task<IEnumerable<IDictionary<string, object>>> GetValues(string key);
}
