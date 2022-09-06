using QuestPDF.Fluent;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export(typeof(IPagePartBuilder))]
public class FooterPartBuilder : IPagePartBuilder
{
    private readonly ContainerBuilder ContainerBuilder;
    private readonly ContainerMutator ContainerMutator;

    public FooterPartBuilder(ContainerBuilder containerBuilder, ContainerMutator containerMutator)
    {
        ContainerBuilder = containerBuilder;
        ContainerMutator = containerMutator;
    }

    public NodeType Type => NodeType.Footer;

    public void Build(Node node, PageDescriptor page)
    {
        var footer = page.Footer();
        footer = ContainerMutator.Mutate(footer, node);
        ContainerBuilder.Build(node, footer);
    }
}