using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export(typeof(IContainerBuilder))]
public class RowBuilder : IContainerBuilder
{
    private readonly ContainerBuilder ContainerBuilder;
    private readonly ContainerMutator ContainerMutator;
    public NodeType Type => NodeType.Row;

    public RowBuilder(ContainerBuilder containerBuilder, ContainerMutator containerMutator)
    {
        ContainerBuilder = containerBuilder;
        ContainerMutator = containerMutator;
    }

    public void Build(Node node, IContainer container)
    {
        ContainerMutator.Mutate(container, node)
        .Row(r =>
        {
            var spacing = node.GetAttributeFloat("spacing");
            if (spacing is not null)
            {
                r.Spacing(spacing.Value);
            }
            ContainerBuilder.Build(node, () => r.RelativeItem());
        });
    }
}