using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export(typeof(IContainerBuilder))]
public class TableHeaderBuilder : TableRowBuilder
{
    public override NodeType Type => NodeType.TableHeader;

    public TableHeaderBuilder(ContainerBuilder containerBuilder, ContainerMutator containerMutator) : base(containerBuilder, containerMutator)
    {
    }
}