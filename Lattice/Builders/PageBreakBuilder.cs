using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export(typeof(IContainerBuilder))]
[AutoConstructor]
public partial class PageBreakBuilder : IContainerBuilder
{
    private readonly ContainerMutator ContainerMutator;

    public NodeType Type => NodeType.PageBreak;

    public void Build(Node node, IContainer container)
    {
        ContainerMutator
        .Mutate(container, node)
        .PageBreak();
    }
}