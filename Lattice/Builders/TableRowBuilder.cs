using QuestPDF.Infrastructure;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export<IContainerBuilder>]
public class TableRowBuilder(
    ContainerBuilder ContainerBuilder,
    ContainerMutator ContainerMutator
)
    : IContainerBuilder
{
    public virtual NodeType Type => NodeType.TableRow;

    public void Build(Node node, IContainer container)
    {
        ContainerBuilder.Build(node, ContainerMutator.Mutate(container, node));
    }
}