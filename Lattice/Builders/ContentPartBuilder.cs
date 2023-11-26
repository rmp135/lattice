using AutoCtor;
using QuestPDF.Fluent;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export(typeof(IPagePartBuilder))]
[AutoConstruct]
public partial class ContentPartBuilder : IPagePartBuilder
{
    private readonly ContainerBuilder ContainerBuilder;
    private readonly ContainerMutator ContainerMutator;

    public NodeType Type => NodeType.Content;

    public void Build(Node node, PageDescriptor page)
    {
        var content = page.Content();
        ContainerBuilder.Build(node, ContainerMutator.Mutate(content, node));
    }
}