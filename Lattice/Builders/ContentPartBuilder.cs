using QuestPDF.Fluent;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export<IPagePartBuilder>]
public class ContentPartBuilder(
    ContainerBuilder ContainerBuilder,
    ContainerMutator ContainerMutator
)
    : IPagePartBuilder
{
    public NodeType Type => NodeType.Content;

    public void Build(Node node, PageDescriptor page)
    {
        var content = page.Content();
        ContainerBuilder.Build(node, ContainerMutator.Mutate(content, node));
    }
}