using System.Text.RegularExpressions;
using Lattice.Nodes;
using Lattice.Sources;

namespace Lattice;

public interface INodeConstructor
{
    /// <summary>
    /// Expands and applies context to the node tree, using an empty source.
    /// </summary>
    Task ConstructAsync(Node node);

    /// <summary>
    /// Expands and applies context to the node tree.
    /// </summary>
    Task ConstructAsync(Node node, ISource source);
}

/// <summary>
/// Expands a node tree, assigning context from a data source.
/// </summary>
[Export<INodeConstructor>]
public partial class NodeConstructor(ContextReplacer ContextReplacer) : INodeConstructor
{
    public Task ConstructAsync(Node node) => ConstructAsync(node, new FakeSource());

    public async Task ConstructAsync(Node node, ISource source)
    {
        ApplyTemplates(node);
        await BindAsync(node, source);

        await ExpandRepeaterAsync(node, childNode => Task.FromResult(ExpandRepeat(childNode)));
        await ExpandRepeaterAsync(node, childNode => ExpandForAsync(childNode, source));
        
        ProcessConditionals(node);
        
        // ReSharper disable once ForCanBeConvertedToForeach
        // Loop may remove items from the list prevent foreach from working.
        for (var i = 0; i < node.ChildNodes.Count; i++)
        {
            var childNode = node.ChildNodes[i];
            await ConstructAsync(childNode, source);
            DissolveVirtualNodes(childNode);
        }
        RemoveTemplates(node);
    }

    /// <summary>
    /// Recursively removes all <see cref="NodeType.Template"/> nodes from a <see cref="Node"/>.
    /// </summary>
    /// <para ></para>
    public void RemoveTemplates(Node node)
    {
        for (var i = 0; i < node.ChildNodes.Count; i++)
        {
            var childNode = node.ChildNodes[i];
            if (childNode.Type == NodeType.Template)
            {
                node.RemoveChild(childNode);
            }
            else
            {
                RemoveTemplates(childNode);
            }
        }
    }

    /// <summary>
    /// Applies all applicable templates to a node, replacing the attributes.
    /// </summary>
    /// <param name="node">The node to apply templates to.</param>
    public void ApplyTemplates(
        Node node
    )
    {
        var templateNames = node.GetAttribute("template")?.Split(" ") ?? Array.Empty<string>();
        if (!templateNames.Any()) return;
        var templateNodes = node.GetApplicableTemplateNodes();
        foreach (var templateName in templateNames)
        {
            var foundTemplate = templateNodes.FirstOrDefault(t => t.GetAttribute("name") == templateName);
            if (foundTemplate is null) continue;
            foreach (var attribute in foundTemplate.Attributes)
            {
                // Don't overwrite the name attribute.
                if (attribute.Key.Equals("name")) continue;
                node.ReplaceAttribute(attribute.Key, attribute.Value);
            }
        }
    }
    /// <summary>
    /// Iterates over all children of a <see cref="Node"/>, replacing the child node with the result of the expansion function.
    /// </summary>
    /// <param name="node">The <see cref="Node"/> to iterate child nodes.</param>
    /// <param name="func">The expansion function returning the nodes to be replaced with.</param>
    public async Task ExpandRepeaterAsync(Node node, Func<Node, Task<IEnumerable<Node>>> func)
    {
        for (var i = 0; i < node.ChildNodes.Count; i++)
        {
            var currentNode = node.ChildNodes[i];
            var expandedNodes = (await func(node.ChildNodes[i])).ToArray();
            node.RemoveChild(currentNode);
            for (var offset = 0; offset < expandedNodes.Length; offset++)
            {
                var expandedNode = expandedNodes[offset];
                node.AddChildAt(expandedNode, i + offset);
            }
            i += expandedNodes.Length - 1;
        }
    }

    /// <summary>
    /// Processes the "if" and "else" attributes..
    /// </summary>
    /// <param name="node">The <see cref="Node"/> to process children of.</param>
    public void ProcessConditionals(
        Node node
    )
    {
        bool? previousIfResult = null;
        for (var i = 0; i < node.ChildNodes.Count; i++)
        {
            var childNode = node.ChildNodes[i];
            var ifAttribute = childNode.GetAttribute("if");
            if (ifAttribute is null)
            {
                var elseAttribute = childNode.GetAttribute("else");
                if (elseAttribute is not null && previousIfResult.HasValue && previousIfResult.Value) 
                {
                    node.ChildNodes.RemoveAt(i);
                    i--;
                }
                previousIfResult = null;
                continue;
            }
            var response = ContextReplacer.ReplaceToken(childNode, $"if({ifAttribute}, true, false)");
            var result = response.Equals("True", StringComparison.OrdinalIgnoreCase);
            previousIfResult = result;
            if (!result)
            {
                node.ChildNodes.RemoveAt(i);
                i--;
            }
        }
    }

    /// <summary>
    /// Removes all <see cref="NodeType.Virtual"/> nodes, re-binding context to the children.
    /// </summary>
    /// <param name="node">The node that contains the virtual nodes.</param>
    public void DissolveVirtualNodes(Node node)
    {
        var newnodes = new List<Node>();
        for (var i = node.ChildNodes.Count - 1; i >= 0; i--)
        {
            var childNode = node.ChildNodes[i];
            if (childNode.Type != NodeType.Virtual)
            {
                newnodes.Add(childNode);
                node.RemoveChild(childNode);
                continue;
            }

            foreach (var grandchildNode in childNode.ChildNodes.Reverse())
            {
                foreach (var k in childNode.Context)
                {
                    grandchildNode.ReplaceContextKey(k.Key, k.Value);
                }

                newnodes.Add(grandchildNode);
            }

            node.RemoveChild(childNode);
        }

        newnodes.Reverse();
        foreach (var newnode in newnodes)
        {
            node.AddChild(newnode);
        }
    }
    
    /// <summary>
    /// Binds context to a <see cref="Node"/> without expanding into a loop.
    /// </summary>
    /// <param name="node">The <see cref="Node"/> to bind context to.</param>
    /// <param name="source">The <see cref="ISource"/> to take context values from.</param>
    public async Task BindAsync(Node node, ISource source)
    {
        var bindStr = node.GetAttribute("bind");
        if (bindStr is null) return;
        var matches = ForLoopRegex().Matches(bindStr);
        if (!matches.Any()) return;
        var contextKey = matches[0].Groups[1].Value;
        var contextValue = matches[0].Groups[2].Value;

        var data = (await source.GetValues(ContextReplacer.ReplaceTokens(contextValue, node))).ToArray();
        node.ReplaceContextKey($"{contextKey}", new GroupedContextValue(contextKey, data));
        // Bind also binds the first item in the array for ease of use.
        var firstResult = data.FirstOrDefault();
        if (firstResult != null)
        {
            foreach (var dataItem in firstResult)
            {
                node.ReplaceContextKey(dataItem.Key, new ObjectContextValue(dataItem.Value));
            }
        }
    }
    
    /// <summary>
    /// Expands a "for x in y" loop using context from a <see cref="Node"/> and <see cref="ISource"/>. Handles grouped and ungrouped data sets.
    /// </summary>
    /// <param name="node">The <see cref="Node"/> to expand.</param>
    /// <param name="source">The <see cref="ISource"/> to take context values from.</param>
    /// <returns></returns>
    public async Task<IEnumerable<Node>> ExpandForAsync(Node node, ISource source)
    {
        var repeatStr = node.GetAttribute("for");
        if (repeatStr is null) return new[] { node };
        var matches = new Regex(@"(.+) in (.+)").Matches(repeatStr);
        if (!matches.Any()) return node.ChildNodes;
        var contextKey = matches[0].Groups[1].Value;
        var contextValue = matches[0].Groups[2].Value;

        var tokens = ContextReplacer.GetTokens(contextValue).ToArray();
        if (tokens.Length == 1 && tokens[0].IsToken)
        {
            var contextdata = node.GetContextValue(tokens[0].Text);
            if (contextdata is null) return node.ChildNodes;
            return ExpandForWithData(node, contextKey, contextdata);
        }

        var data = await source.GetValues(ContextReplacer.ReplaceTokens(contextValue, node));
        return ExpandForWithData(node, contextKey, data);
    }

    /// <summary>
    /// Expands a for loop, binding the data and ordering the nodes.
    /// </summary>
    /// <param name="node">The <see cref="Node"/> to expand.</param>
    /// <param name="contextKey">The context key of for the "for x in {context}" attribute.</param>
    /// <param name="data">The data from either the <see cref="ISource"/> or a <see cref="GroupedContextValue"/>.</param>
    /// <returns>Either the new expanded nodes, the the child nodes of the input node.</returns>
    public IEnumerable<Node> ExpandForWithData(Node node, string contextKey, IEnumerable<IDictionary<string, object>> data)
    {
        node.RemoveAttribute("for");
        
        var orderByAsc = node.GetAttribute("orderByAsc");
        var orderByDesc = node.GetAttribute("orderByDesc");

        var dataArr = data.ToArray();

        var containsKey = (IEnumerable<IDictionary<string, object>> dict, string key) =>
        {
            if (!dict.Any()) return false;
            return dict.FirstOrDefault().Select(s => s.Key).Contains(key);
        };
        
        if (!string.IsNullOrEmpty(orderByAsc) && containsKey(dataArr, orderByAsc))
        {
            dataArr = dataArr.OrderBy(a => a[orderByAsc]).ToArray();
        }
        else if (!string.IsNullOrEmpty(orderByDesc) && containsKey(dataArr, orderByDesc))
        {
            dataArr = dataArr.OrderByDescending(a => a[orderByDesc]).ToArray();
        }
            
        var newNodes = new List<Node>();
        
        var groupBy = node.GetAttribute("groupBy");
        if (!string.IsNullOrEmpty(groupBy) && containsKey(dataArr, groupBy))
        {
            var grouped = dataArr
                .GroupBy(d => d[groupBy])
                .ToDictionary(group => group.Key, group => group)
                .ToArray();

            for (var i = 0; i < grouped.Length; i++)
            {
                var group = grouped[i];
                var newNode = node.DeepClone();

                if (group.Value.Any())
                {
                    foreach (var key in group.Value.First())
                    {
                        newNode.Context.Add($"{contextKey}.{key.Key}", new ObjectContextValue(key.Value));
                    }
                }

                newNode.ReplaceContextKey(contextKey, new GroupedContextValue(contextKey, group.Value));
                newNode.ReplaceContextKey($"{contextKey}.index", new ObjectContextValue(i));

                newNodes.Add(newNode);
            }

        }

        // Not grouped.
        else
        {
            for (var i = 0; i < dataArr.Length; i++)
            {
                var row = dataArr[i];
                var newNode = node.DeepClone();
                newNode.Context.Add($"{contextKey}", new GroupedContextValue(contextKey, dataArr));
                foreach (var key in row.Keys)
                {
                    newNode.Context.Add($"{contextKey}.{key}", new ObjectContextValue(row[key]));
                    newNode.Context.Remove($"{contextKey}.$index");
                    newNode.Context.Add($"{contextKey}.$index", new ObjectContextValue(i));
                }

                newNodes.Add(newNode);
            }
        }

        return newNodes;
    }


    /// <summary>
    /// Expands a "repeat" loop, deep cloning the <see cref="Node"/> a number of times.
    /// </summary>
    /// <param name="node">The <see cref="Node"/> to repeat.</param>
    /// <returns>Either the new expanded nodes, the the child nodes of the input node.</returns>
    public IEnumerable<Node> ExpandRepeat(Node node)
    {
        var repeatStr = node.GetAttribute("repeat");
        if (repeatStr is null || !int.TryParse(repeatStr, out var repeat)) return new[] { node };
        node.RemoveAttribute("repeat");
        var newNodes = new List<Node>();
        for (var i = 0; i < repeat; i++)
        {
            var newNode = node.DeepClone();
            newNode.Context.Remove("$index");
            newNode.Context.Add("$index", new ObjectContextValue(i));
            newNodes.Add(newNode);
        }
        return newNodes;
    }

    [GeneratedRegex(@"(.+) in (.+)")]
    private static partial Regex ForLoopRegex();
}
