using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export(typeof(IContainerBuilder))]
public class PageBreakBuilder : IContainerBuilder
{
    private readonly ContainerMutator ContainerMutator;

    public NodeType Type => NodeType.PageBreak;

    public PageBreakBuilder(ContainerMutator containerMutator)
    {
        ContainerMutator = containerMutator;
    }

    public void Build(Node node, IContainer container)
    {
        ContainerMutator
        .Mutate(container, node)
        .PageBreak();
    }
}