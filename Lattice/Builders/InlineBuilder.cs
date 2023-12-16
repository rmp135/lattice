using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export<IContainerBuilder>]
public class InlinedBuilder(
    ContainerBuilder ContainerBuilder,
    ContainerMutator ContainerMutator
)
    : IContainerBuilder
{
    public NodeType Type => NodeType.Inline;

    public void Build(Node node, IContainer container)
    {
        ContainerMutator.Mutate(container, node)
            .Inlined(i =>
        {
            ContainerBuilder.Build(node, i.Item);
        });
    }
}