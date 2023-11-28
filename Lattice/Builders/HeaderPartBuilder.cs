using QuestPDF.Fluent;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export(typeof(IPagePartBuilder))]
[AutoConstructor]
public partial class HeaderPartBuilder : IPagePartBuilder
{
    private readonly ContainerBuilder ContainerBuilder;
    private readonly ContainerMutator ContainerMutator;

    public NodeType Type => NodeType.Header;

    public void Build(Node node, PageDescriptor page)
    {
        var header = page.Header();
        header = ContainerMutator.Mutate(header, node);
        ContainerBuilder.Build(node, header);
    }
}