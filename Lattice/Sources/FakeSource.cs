namespace Lattice.Sources;

/// <summary>
/// A source that always returns an empty record set.
/// </summary>
internal class FakeSource: ISource
{
    Task<IEnumerable<IDictionary<string, object>>> ISource.GetValues(string key)
    {
        return Task.FromResult(Enumerable.Empty<IDictionary<string, object>>());
    }
}