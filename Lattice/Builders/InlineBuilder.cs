using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export(typeof(IContainerBuilder))]
public class InlinedBuilder : IContainerBuilder
{
    private readonly ContainerBuilder ContainerBuilder;
    private readonly ContainerMutator ContainerMutator;

    public NodeType Type => NodeType.Inline;

    public InlinedBuilder(ContainerBuilder containerBuilder, ContainerMutator containerMutator)
    {
        ContainerBuilder = containerBuilder;
        ContainerMutator = containerMutator;
    }

    public void Build(Node node, IContainer container)
    {
        ContainerMutator.Mutate(container, node)
            .Inlined(i =>
        {
            ContainerBuilder.Build(node, i.Item);
        });
    }
}