using Lattice.AttributeMutators;

namespace Lattice.Builders;

[Export<IContainerBuilder>]
public class TableFooterBuilder(ContainerBuilder ContainerBuilder, ContainerMutator ContainerMutator) : TableRowBuilder(ContainerBuilder, ContainerMutator)
{
    public override NodeType Type => NodeType.TableFooter;
}