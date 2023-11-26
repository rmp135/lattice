using AutoCtor;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export(typeof(IContainerBuilder))]
[AutoConstruct]
public partial class TableHeaderBuilder : TableRowBuilder
{
    public override NodeType Type => NodeType.TableHeader;
}