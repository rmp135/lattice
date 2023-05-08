using System.Collections;
using System.Globalization;
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

[Export(typeof(INodeConstructor))]
public class NodeConstructor : INodeConstructor
{
    private readonly ContextReplacer ContextReplacer;
    private readonly ExpressionHelper ExpressionHelper;

    /// <summary>
    /// Expands a node tree, assigning context from a data source.
    /// </summary>
    public NodeConstructor(
        ContextReplacer contextReplacer,
        ExpressionHelper expressionHelper
    )
    {
        ContextReplacer = contextReplacer;
        ExpressionHelper = expressionHelper;
    }

    public Task ConstructAsync(Node node) => ConstructAsync(node, new FakeSource());

    public async Task ConstructAsync(Node node, ISource source)
    {
        await BindAsync(node, source);

        await ExpandRepeaterAsync(node, childNode => Task.FromResult(ExpandRepeat(childNode)));
        await ExpandRepeaterAsync(node, childNode => ExpandForAsync(childNode, source));
        
        ProcessIfNodes(node);
        
        for (var i = 0; i < node.ChildNodes.Count; i++)
        {
            var childNode = node.ChildNodes[i];
            await ConstructAsync(childNode, source);
            DissolveVirtualNodes(childNode);
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
            if (expandedNodes.Length == 1) continue;
            node.RemoveChild(currentNode);
            foreach (var expandedNode in expandedNodes)
            {
                node.AddChildAt(expandedNode, i);
            }
            i += expandedNodes.Length - 1;
        }

    }

    /// <summary>
    /// Processes the "if" attribute, removing the node of the expression is false.
    /// </summary>
    /// <param name="node">The <see cref="Node"/> to process children of.</param>
    public void ProcessIfNodes(
        Node node
    )
    {
        for (var i = 0; i < node.ChildNodes.Count; i++)
        {
            var attribute = node.GetAttribute("if");
            if (attribute is null) return;
            var response = ExpressionHelper.RunExpression(node, $"if({attribute}, true, false)");
            if (response == "True")
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
        var childNodes = node.ChildNodes.Where(a => a.Type == NodeType.Virtual).ToArray();
        foreach (var childNode in childNodes)
        {
            foreach (var grandchildNode in childNode.ChildNodes)
            {
                foreach (var k in childNode.Context)
                {
                    grandchildNode.ReplaceContextKey(k.Key, k.Value);
                }

                node.AddChild(grandchildNode);
                node.RemoveChild(childNode);
            }
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
        var matches = new Regex(@"(.+) in (.+)").Matches(bindStr);
        if (!matches.Any()) return;
        var contextKey = matches[0].Groups[1].Value;
        var contextValue = matches[0].Groups[2].Value;

        var data = await source.GetValues(ContextReplacer.ReplaceTokens(contextValue, node));
        node.ReplaceContextKey($"{contextKey}", new GroupedContextValue(contextKey, data.ToArray()));
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
        var parentNode = node.ParentNode;
        
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
                        newNode.Context.Add($"{contextKey}.{key.Key}",
                            new StringContextValue(key.Value?.ToString() ?? ""));
                    }
                }

                newNode.ReplaceContextKey(contextKey, new GroupedContextValue(contextKey, group.Value));
                newNode.ReplaceContextKey($"{contextKey}.index", new StringContextValue(i.ToString()));

                node.ParentNode!.AddChildAt(newNode, node.ParentNode!.ChildNodes.IndexOf(node));
            }

            node.ParentNode.RemoveChild(node);
            return parentNode.ChildNodes;
        }

        // Not grouped.
        for (var i = 0; i < dataArr.Length; i++)
        {
            var row = dataArr[i];
            var newNode = node.DeepClone();
            newNode.Context.Add($"{contextKey}", new GroupedContextValue(contextKey, dataArr));
            foreach (var key in row.Keys)
            {
                newNode.Context.Add($"{contextKey}.{key}", new StringContextValue(row[key].ToString() ?? ""));
                newNode.Context.Remove($"{contextKey}.index");
                newNode.Context.Add($"{contextKey}.index", new StringContextValue(i.ToString()));
            }

            node.ParentNode!.AddChildAt(newNode, node.ParentNode!.ChildNodes.IndexOf(node));
        }

        node.ParentNode.RemoveChild(node); 
        return parentNode.ChildNodes;
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
            newNode.Context.Remove("index");
            newNode.Context.Add("index", new StringContextValue(i.ToString()));
            newNodes.Add(newNode);
        }
        return newNodes;
    }
}