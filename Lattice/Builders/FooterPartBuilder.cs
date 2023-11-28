using QuestPDF.Fluent;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export(typeof(IPagePartBuilder))]
[AutoConstructor]
public partial class FooterPartBuilder : IPagePartBuilder
{
    private readonly ContainerBuilder ContainerBuilder;
    private readonly ContainerMutator ContainerMutator;

    public NodeType Type => NodeType.Footer;

    public void Build(Node node, PageDescriptor page)
    {
        var footer = page.Footer();
        footer = ContainerMutator.Mutate(footer, node);
        ContainerBuilder.Build(node, footer);
    }
}