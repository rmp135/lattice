using QuestPDF.Fluent;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export<IPagePartBuilder>]
public class HeaderPartBuilder(
    ContainerBuilder ContainerBuilder,
    ContainerMutator ContainerMutator
)
    : IPagePartBuilder
{
    public NodeType Type => NodeType.Header;

    public void Build(Node node, PageDescriptor page)
    {
        var header = page.Header();
        header = ContainerMutator.Mutate(header, node);
        ContainerBuilder.Build(node, header);
    }
}