using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export(typeof(IContainerBuilder))]
public class ColumnBuilder : IContainerBuilder
{
    private readonly ContainerBuilder ContainerBuilder;
    private readonly ContainerMutator ContainerMutator;

    public NodeType Type => NodeType.Column;

    public ColumnBuilder(ContainerBuilder containerBuilder, ContainerMutator containerMutator)
    {
        ContainerBuilder = containerBuilder;
        ContainerMutator = containerMutator;
    }

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