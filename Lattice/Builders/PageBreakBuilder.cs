using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export<IContainerBuilder>]
public class PageBreakBuilder(ContainerMutator ContainerMutator) : IContainerBuilder
{
    public NodeType Type => NodeType.PageBreak;

    public void Build(Node node, IContainer container)
    {
        ContainerMutator
        .Mutate(container, node)
        .PageBreak();
    }
}