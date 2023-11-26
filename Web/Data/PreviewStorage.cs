using Microsoft.Extensions.Caching.Memory;

namespace Lattice.Web.Data;

/// <summary>
/// Stores a value in memory for a short period of time.
/// Used for previewing as a POST request can't be made from the editor.
/// </summary>
public class PreviewStorage
{
    private readonly IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());

    public string AddToStore(string value)
    {
        var id = Guid.NewGuid().ToString("N");
        cache.Set(id, value, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
        });
        return id;
    }

    public string? Get(string id)
    {
        return cache.Get<string>(id);
    }
}
