namespace Lattice.Builders;

[Export(typeof(IContainerBuilder))]
[AutoConstructor]
public partial class TableFooterBuilder : TableRowBuilder
{
    public override NodeType Type => NodeType.TableFooter;
}