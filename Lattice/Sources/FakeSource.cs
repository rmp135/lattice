namespace Lattice.Sources;

internal class FakeSource: ISource
{
    Task<IEnumerable<IDictionary<string, object>>> ISource.GetValues(string key)
    {
        return Task.FromResult(Enumerable.Empty<IDictionary<string, object>>());
    }
}