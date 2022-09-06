using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export(typeof(IContainerBuilder))]
public class TableFooterBuilder : TableRowBuilder
{
    public override NodeType Type => NodeType.TableFooter;

    public TableFooterBuilder(ContainerBuilder containerBuilder, ContainerMutator containerMutator) : base(containerBuilder, containerMutator)
    {
    }
}