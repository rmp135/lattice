using QuestPDF.Fluent;
using QuestPDF.Helpers;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export(typeof(IPagePartBuilder))]
public class HeaderPartBuilder : IPagePartBuilder
{
    private readonly ContainerBuilder ContainerBuilder;
    private readonly ContainerMutator ContainerMutator;

    public HeaderPartBuilder(ContainerBuilder containerBuilder, ContainerMutator containerMutator)
    {
        ContainerBuilder = containerBuilder;
        ContainerMutator = containerMutator;
    }

    public NodeType Type => NodeType.Header;

    public void Build(Node node, PageDescriptor page)
    {
        var header = page.Header();
        header = ContainerMutator.Mutate(header, node);
        ContainerBuilder.Build(node, header);
    }
}