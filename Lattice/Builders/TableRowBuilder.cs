using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export(typeof(IContainerBuilder))]
public class TableRowBuilder : IContainerBuilder
{
    private readonly ContainerBuilder ContainerBuilder;
    private readonly ContainerMutator ContainerMutator;

    public virtual NodeType Type => NodeType.TableRow;

    public TableRowBuilder(ContainerBuilder containerBuilder, ContainerMutator containerMutator)
    {
        ContainerBuilder = containerBuilder;
        ContainerMutator = containerMutator;
    }

    public void Build(Node node, IContainer container)
    {
        ContainerBuilder.Build(node, ContainerMutator.Mutate(container, node));
    }
}