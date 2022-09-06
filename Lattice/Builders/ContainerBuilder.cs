using System.Runtime.InteropServices.ComTypes;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export(typeof(ContainerBuilder))]
public class ContainerBuilder
{
    private readonly IServiceProvider ServiceProvider;

    public ContainerBuilder(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public void Build(Node node, Func<IContainer> containerFunc)
    {
        foreach (var childNode in node.ChildNodes)
        {
            BuildSingleNode(childNode, containerFunc);
        }
    }

    private void BuildSingleNode(Node node, Func<IContainer> containerFunc)
    {
        // Because of the recursive nature of the builders, we must defer loading.
        var containerBuilders = ServiceProvider.GetRequiredService<IEnumerable<IContainerBuilder>>().ToArray();
        var builder = containerBuilders.FirstOrDefault(c => c.Type == node.Type);
        builder?.Build(node, containerFunc());
    }

    public void Build(Node node, IContainer container)
    {
        Build(node, () => container);
    }

    public void BuildSingleNode(Node node, IContainer container)
    {
        BuildSingleNode(node, () => container);
    }
}