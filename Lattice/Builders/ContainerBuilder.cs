using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export<ContainerBuilder>]
public class ContainerBuilder(
    IServiceProvider ServiceProvider,
    IEnumerable<ILatticePlugin> Plugins
)
{
    private readonly Lazy<IEnumerable<IContainerBuilder>> ContainerBuilders = new(ServiceProvider.GetServices<IContainerBuilder>);

    // Because of the self-referencing nature of the builders, we must lazy load them.

    /// <summary>
    /// Recursively builds a <see cref="Node"/>, using a factory function for the container. Used when each child must sit inside a different container supplied by the parent. 
    /// </summary>
    /// <param name="node">The <see cref="Node"/> to recursively build</param>
    /// <param name="containerFunc">The factory function for creating each child container.</param>
    public void Build(Node node, Func<IContainer> containerFunc)
    {
        foreach (var childNode in node.ChildNodes)
        {
            BuildSingleNode(childNode, containerFunc);
        }
    }
    
    /// <summary>
    /// Builds a single <see cref="Node"/>, using a factory function for the container that contains it. 
    /// </summary>
    /// <param name="node">The <see cref="Node"/> to build</param>
    /// <param name="containerFunc">The factory function for the container.</param>
    private void BuildSingleNode(Node node, Func<IContainer> containerFunc)
    {
        var builder = ContainerBuilders.Value.FirstOrDefault(c => c.Type == node.Type);
        builder?.Build(node, containerFunc());
        var plugin = Plugins.FirstOrDefault(p => p.Tag.Equals(node.Tag, StringComparison.OrdinalIgnoreCase));
        plugin?.Build(node, containerFunc());
    }

    /// <summary>
    /// Recursively builds a <see cref="Node"/>, using a single instance of a <see cref="IContainer"/> for each child. 
    /// </summary>
    /// <param name="node">The <see cref="Node"/> to recursively build</param>
    /// <param name="container">The <see cref="IContainer"/> the child <see cref="Node"/>s are contained within.</param>
    public void Build(Node node, IContainer container)
    {
        Build(node, () => container);
    }

    /// <summary>
    /// Builds a single <see cref="Node"/>, using a single instance of a <see cref="IContainer"/> for each child. 
    /// </summary>
    /// <param name="node">The <see cref="Node"/> to build</param>
    /// <param name="container">The <see cref="IContainer"/> the <see cref="Node"/> is contained within.</param>
    public void BuildSingleNode(Node node, IContainer container)
    {
        BuildSingleNode(node, () => container);
    }
}