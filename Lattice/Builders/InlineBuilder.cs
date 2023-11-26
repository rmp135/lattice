using AutoCtor;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export(typeof(IContainerBuilder))]
[AutoConstruct]
public partial class InlinedBuilder : IContainerBuilder
{
    private readonly ContainerBuilder ContainerBuilder;
    private readonly ContainerMutator ContainerMutator;

    public NodeType Type => NodeType.Inline;

    public void Build(Node node, IContainer container)
    {
        ContainerMutator.Mutate(container, node)
            .Inlined(i =>
        {
            ContainerBuilder.Build(node, i.Item);
        });
    }
}