using System.Collections;

namespace Lattice.Nodes;

public class Node
{
    public string Tag { get; set; }
    public NodeType Type { get; }
    public IList<Node> ChildNodes { get; } = new List<Node>();
    public Node? ParentNode { get; private set; }

    public Guid ID { get; } = Guid.NewGuid();

    /// <summary>
    /// The bound context that belongs to this <see cref="Node"/>.
    /// </summary>
    public IDictionary<string, ContextValue> Context { get; private init; } = new Dictionary<string, ContextValue>(StringComparer.InvariantCultureIgnoreCase);

    /// <summary>
    /// The list of attributes that belong to this <see cref="Node"/>.
    /// </summary>
    public IList<KeyValuePair<string, string>> Attributes { get; private set; } = new List<KeyValuePair<string, string>>();

    public Node(string tag)
    {
        Tag = tag;
        Type = Enum.TryParse<NodeType>(tag, true, out var type) ? type : NodeType.Plugin;
    }
    
    public Node(NodeType type)
    {
        Type = type;
        Tag = type.ToString();
    }

    /// <summary>
    /// Adds a child to the current <see cref="Node"/>, binding the parent.
    /// </summary>
    /// <param name="node">The child <see cref="Node"/> to add.</param>
    public Node AddChild(Node node)
    {
        node.ParentNode = this;
        ChildNodes.Add(node);
        return this;
    }

    /// <summary>
    /// Adds a child to the current <see cref="Node"/> at a given index.
    /// </summary>
    /// <param name="node">The child <see cref="Node"/> to add.</param>
    /// <param name="index">The index location to add the child.</param>
    public Node AddChildAt(Node node, int index)
    {
        node.ParentNode = this;
        ChildNodes.Insert(index, node);
        return this;
    }

    /// <summary>
    /// Removes a given <see cref="Node"/> from the <see cref="ChildNodes"/>, also removes the parent link.
    /// </summary>
    /// <param name="node">The child node to remove.</param>
    public Node RemoveChild(Node node)
    {
        node.ParentNode = null;
        ChildNodes.Remove(node);
        return this;
    }
    
    /// <summary>
    /// Adds an attribute to the <see cref="Node"/>.
    /// </summary>
    /// <param name="key">The attribute key.</param>
    /// <param name="value">The attribute value.</param>
    public Node AddAttribute(string key, string value)
    {
        Attributes.Add(new KeyValuePair<string, string>(key, value));
        return this;
    }
    
    /// <summary>
    /// Replaces an <see cref="Context"/> item by removing it by key and adding a new one.
    /// </summary>
    /// <param name="key">The key to replace.</param>
    /// <param name="value">The new context value.</param>
    public Node ReplaceContextKey(string key, ContextValue value)
    {
        Context.Remove(key);
        Context.Add(key, value);
        return this;
    }
    
    /// <summary>
    /// Removes a given attribute by key.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    public Node RemoveAttribute(string key)
    {
        Attributes = Attributes.Where(a => !a.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).ToList();
        return this;
    }

    /// <summary>
    /// Retrieves the last attribute of a given key.
    /// </summary>
    /// <param name="key">The key of the attribute to find.</param>
    /// <returns>The last found attribute of that key.</returns>
    public virtual string? GetAttribute(string key)
    {
        return Attributes
            .LastOrDefault(c => c.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase))
            .Value;
    }

    /// <summary>
    /// Attempts to parse <see cref="GetAttribute"/> as a float.
    /// </summary>
    /// <param name="key">The key of the attribute to convert.</param>
    /// <returns></returns>
    public float? GetAttributeFloat(string key)
    {
        var asString = GetAttribute(key);
        var tryParse = float.TryParse(asString, out var result);
        if (!tryParse) return null;
        return result;
    }

    /// <summary>
    /// Attempts to parse <see cref="GetAttribute"/> as an int.
    /// </summary>
    /// <param name="key">The key of the attribute to convert.</param>
    /// <returns></returns>
    public int? GetAttributeInt(string key)
    {
        var asString = GetAttribute(key);
        var tryParse = int.TryParse(asString, out var result);
        if (!tryParse) return null;
        return result;
    }

    /// <summary>
    /// Retrieves a value from the <see cref="Context"/>, or traverses back to find one. 
    /// </summary>
    /// <param name="key">The context key.</param>
    /// <returns>The found context value, or null if one does not exist.</returns>
    public ContextValue? GetContextValue(string key)
    {
        return Context.TryGetValue(key, out var value) ? value : ParentNode?.GetContextValue(key);
    }

    public Dictionary<string, ContextValue> GetAllContextValues()
    {
        var values = Context.ToDictionary(contextValue => contextValue.Key, contextValue => contextValue.Value);
        if (ParentNode == null) return values;
        foreach (var contextValue in ParentNode.GetAllContextValues())
        {
            values.TryAdd(contextValue.Key, contextValue.Value);
        }

        return values;
    }

    /// <summary>
    /// Clones a <see cref="Node"/>, and all properties of that <see cref="Node"/>.
    /// </summary>
    /// <returns>The cloned <see cref="Node"/>.</returns>
    public Node DeepClone()
    {
        var node = new Node(Tag)
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

    public virtual IEnumerable<Node> GetApplicableTemplateNodes()
    {
        return ParentNode == null 
            ? Enumerable.Empty<Node>()
            : ParentNode.ChildNodes
                .Where(c => c.Type == NodeType.Template)
                .Concat(ParentNode.GetApplicableTemplateNodes());
    }
}

/// <summary>
/// A base context value consisting of a string key and object value.
/// </summary>
public abstract class ContextValue :IEnumerable<IDictionary<string, object>>
{
    public abstract IEnumerator<IDictionary<string, object>> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

/// <summary>
/// A context that has been grouped by a particular key. The grouped values can be enumerated.
/// </summary>
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

/// <summary>
/// An ungrouped context value. Attempting to enumerate the values will enumerate the string chars.
/// </summary>
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