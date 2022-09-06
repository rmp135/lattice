using System.Text.RegularExpressions;
using Lattice.Nodes;
using Lattice.Sources;
using org.matheval.Functions;

namespace Lattice;

[Export(typeof(NodeConstructor))]
public class NodeConstructor
{
    private readonly ContextReplacer ContextReplacer;

    public NodeConstructor(ContextReplacer contextReplacer)
    {
        ContextReplacer = contextReplacer;
    }

    public Task ConstructAsync(Node node) => ConstructAsync(node, new FakeSource());

    /// <summary>Expands and applies context to the node tree.</summary>
    public async Task ConstructAsync(Node node, ISource source)
    {
        await BindAsync(node, source);
        // Loops remove the current node and replace them with a set.
        var childNodes = ExpandRepeat(node);
        if (ReferenceEquals(childNodes, node.ChildNodes)) // If the child nodes are returned, no manipulation took place.
        {
            childNodes = (await ExpandForAsync(node, source)).ToArray();
        }

        foreach (var childNode in childNodes)
        {
            await ConstructAsync(childNode, source);
            DissolveVirtualNodes(childNode);
        }
    }

    private void DissolveVirtualNodes(Node node)
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
    
    private async Task BindAsync(Node node, ISource source)
    {
        var bindStr = node.GetAttribute("bind");
        if (bindStr is null) return;
        var matches = new Regex(@"(.+) in (.+)").Matches(bindStr);
        if (!matches.Any()) return;
        var contextKey = matches[0].Groups[1].Value;
        var contextValue = matches[0].Groups[2].Value;

        var data = await source.GetValues(ContextReplacer.ReplaceTokens(contextValue, node));
        var dataArr = data.ToArray();
        node.ReplaceContextKey($"{contextKey}", new GroupedContextValue(contextKey, dataArr));
    }
    
    private async Task<IEnumerable<Node>> ExpandForAsync(Node node, ISource source)
    {
        var repeatStr = node.GetAttribute("for");
        if (repeatStr is null) return node.ChildNodes;
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

    private IEnumerable<Node> ExpandForWithData(Node node, string contextKey, IEnumerable<IDictionary<string, object>> data)
    {
        node.RemoveAttribute("for");
        var parentNode = node.ParentNode;
        
        var orderByAsc = node.GetAttribute("orderByAsc");
        var orderByDesc = node.GetAttribute("orderByDesc");

        var dataArr = data;

        var containsKey = (IEnumerable<IDictionary<string, object>> d, string key) =>
        {
            if (!d.Any()) return false;
            return d.FirstOrDefault().Select(s => s.Key).Contains(key);
        };
        
        if (!string.IsNullOrEmpty(orderByAsc) && containsKey(dataArr, orderByAsc))
        {
            dataArr = dataArr.OrderBy(a => a[orderByAsc]);
        }
        if (!string.IsNullOrEmpty(orderByDesc) && containsKey(dataArr, orderByDesc))
        {
            dataArr = dataArr.OrderByDescending(a => a[orderByDesc]);
        }

        dataArr = dataArr.ToArray();

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
        for (var i = 0; i < dataArr.Count(); i++)
        {
            var row = dataArr.ElementAt(i);
            var newNode = node.DeepClone();
            newNode.Context.Add($"{contextKey}", new GroupedContextValue(contextKey, dataArr));
            foreach (var key in row.Keys)
            {
                newNode.Context.Add($"{contextKey}.{key}", new StringContextValue(row[key]?.ToString() ?? ""));
                newNode.Context.Remove($"{contextKey}.index");
                newNode.Context.Add($"{contextKey}.index", new StringContextValue(i.ToString()));
            }

            node.ParentNode!.AddChildAt(newNode, node.ParentNode!.ChildNodes.IndexOf(node));
        }

        node.ParentNode.RemoveChild(node);
        return parentNode.ChildNodes;
    }


    private IEnumerable<Node> ExpandRepeat(Node node)
    {
        var repeatStr = node.GetAttribute("repeat");
        if (repeatStr is null || !int.TryParse(repeatStr, out var repeat)) return node.ChildNodes;
        node.RemoveAttribute("repeat");
        for (var i = 0; i < repeat; i++)
        {
            var newNode = node.DeepClone();
            newNode.Context.Remove("index");
            newNode.Context.Add("index", new StringContextValue(i.ToString()));
            node.ParentNode!.ChildNodes.Insert(node.ParentNode!.ChildNodes.IndexOf(node), newNode);
        }
        var pareneNode = node.ParentNode;
        node.ParentNode.RemoveChild(node);
        return pareneNode.ChildNodes;
    }
}