namespace Lattice.Builders;

[Export(typeof(IContainerBuilder))]
[AutoConstructor]
public partial class TableHeaderBuilder : TableRowBuilder
{
    public override NodeType Type => NodeType.TableHeader;
}