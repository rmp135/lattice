namespace Lattice.Web.Data;

public class TempStorage
{
    private IList<StoreItem> storage = new List<StoreItem>();

    public string AddToStore(string value)
    {
        storage = storage.Where(s => s.CreatedDate.CompareTo(DateTime.Now.AddMinutes(-1)) < 0).ToList();
        var newItem = new StoreItem(value);
        storage.Add(newItem);
        return newItem.ID;
    }

    public string? Get(string id)
    {
        return storage.FirstOrDefault(s => s.ID == id)?.Value;
    }
}

public class StoreItem
{
    public DateTime CreatedDate { get; } = DateTime.Now;
    public string ID { get; } = Guid.NewGuid().ToString("N");
    public string Value { get; }

    public StoreItem(string value)
    {
        Value = value;
    }
}