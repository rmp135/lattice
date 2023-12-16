using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export<IContainerBuilder>]
public class ColumnBuilder(
    ContainerBuilder ContainerBuilder,
    ContainerMutator ContainerMutator
)
    : IContainerBuilder
{
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