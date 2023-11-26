using AutoCtor;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export(typeof(IContainerBuilder))]
[AutoConstruct]
public partial class ColumnBuilder : IContainerBuilder
{
    private readonly ContainerBuilder ContainerBuilder;
    private readonly ContainerMutator ContainerMutator;

    public NodeType Type => NodeType.Column;

    public void Build(Node node, IContainer container)
    {
        ContainerMutator
            .Mutate(container, node)
            .Column(c =>
        {
            var spacing = node.GetAttributeFloat("spacing");
            if (spacing is not null)
            {
                c.Spacing(spacing.Value);
            }
            ContainerBuilder.Build(node, c.Item);
        });
    }
}