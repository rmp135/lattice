using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Lattice.Nodes;

public class Node
{
    public NodeType Type { get; }
    public IList<Node> ChildNodes { get; } = new List<Node>();
    public Node? ParentNode { get; private set; }

    public Guid ID { get; } = Guid.NewGuid();

    public IDictionary<string, ContextValue> Context { get; private init; } = new Dictionary<string, ContextValue>();

    public IList<KeyValuePair<string, string>> Attributes { get; private set; } = new List<KeyValuePair<string, string>>();

    public Node(NodeType type)
    {
        Type = type;
    }

    public Node AddChild(Node node)
    {
        node.ParentNode = this;
        ChildNodes.Add(node);
        return this;
    }

    public Node AddChildAt(Node node, int index)
    {
        node.ParentNode = this;
        ChildNodes.Insert(index, node);
        return this;
    }

    public Node RemoveChild(Node node)
    {
        node.ParentNode = null;
        ChildNodes.Remove(node);
        return this;
    }

    public Node AddAttribute(string key, string value)
    {
        Attributes.Add(new KeyValuePair<string, string>(key, value));
        return this;
    }

    public Node ReplaceContextKey(string key, ContextValue value)
    {
        Context.Remove(key);
        Context.Add(key, value);
        return this;
    }

    public Node RemoveAttribute(string key)
    {
        Attributes = Attributes.Where(a => !a.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).ToList();
        return this;
    }

    public string? GetAttribute(string key)
    {
        return Attributes
            .LastOrDefault(c => c.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase))
            .Value;
    }

    public float? GetAttributeFloat(string key)
    {
        var asString = GetAttribute(key);
        var tryParse = float.TryParse(asString, out var result);
        if (!tryParse) return null;
        return result;
    }

    public int? GetAttributeInt(string key)
    {
        var asString = GetAttribute(key);
        var tryParse = int.TryParse(asString, out var result);
        if (!tryParse) return null;
        return result;
    }

    public ContextValue? GetContextValue(string key)
    {
        return Context.TryGetValue(key, out var value) ? value : ParentNode?.GetContextValue(key);
    }

    public Node DeepClone()
    {
        var node = new Node(Type)
        {
            Attributes = new List<KeyValuePair<string, string>>(Attributes),
            Context = new Dictionary<string, ContextValue>(Context),
            ParentNode = ParentNode
        };
        foreach (var childNode in ChildNodes.Select(c => c.DeepClone()))
        {
            node.AddChild(childNode);
        }
        return node;
    }
}

public abstract class ContextValue :IEnumerable<IDictionary<string, object>>
{
    public abstract IEnumerator<IDictionary<string, object>> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class GroupedContextValue : ContextValue
{
    private readonly IEnumerable<IDictionary<string, object>> data;
    private readonly string key;

    public GroupedContextValue(string key, IEnumerable<IDictionary<string, object>> data)
    {
        this.key = key;
        this.data = data;
    }

    public override IEnumerator<IDictionary<string, object>> GetEnumerator()
    {
        return data.GetEnumerator();
    }

    public override string ToString()
    {
        return $"{{{key} set. Count: {data.Count()}}}";
    }
}

public class StringContextValue : ContextValue
{
    private readonly string value;

    public StringContextValue(string value)
    {
        this.value = value;
    }

    public override IEnumerator<IDictionary<string, object>> GetEnumerator()
    {
        foreach (var c in value)
        {
            yield return new Dictionary<string, object>{["value"] = c};
        }
    }

    public override string ToString()
    {
        return value;
    }
}