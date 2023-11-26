using AutoCtor;
using QuestPDF.Infrastructure;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export(typeof(IContainerBuilder))]
[AutoConstruct]
public partial class TableRowBuilder : IContainerBuilder
{
    private readonly ContainerBuilder ContainerBuilder;
    private readonly ContainerMutator ContainerMutator;

    public virtual NodeType Type => NodeType.TableRow;

    public void Build(Node node, IContainer container)
    {
        ContainerBuilder.Build(node, ContainerMutator.Mutate(container, node));
    }
}