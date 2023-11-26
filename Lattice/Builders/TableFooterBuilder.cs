using AutoCtor;

namespace Lattice.Builders;

[Export(typeof(IContainerBuilder))]
[AutoConstruct]
public partial class TableFooterBuilder : TableRowBuilder
{
    public override NodeType Type => NodeType.TableFooter;
}