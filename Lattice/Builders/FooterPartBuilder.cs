using QuestPDF.Fluent;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export<IPagePartBuilder>]
public class FooterPartBuilder(
    ContainerBuilder ContainerBuilder,
    ContainerMutator ContainerMutator
)
    : IPagePartBuilder
{
    public NodeType Type => NodeType.Footer;

    public void Build(Node node, PageDescriptor page)
    {
        var footer = page.Footer();
        footer = ContainerMutator.Mutate(footer, node);
        ContainerBuilder.Build(node, footer);
    }
}