namespace Lattice.Sources;

public interface ISource
{
    public Task<IEnumerable<IDictionary<string, object>>> GetValues(string key);
}
