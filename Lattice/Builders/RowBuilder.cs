using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export(typeof(IContainerBuilder))]
[AutoConstructor]
public partial class RowBuilder : IContainerBuilder
{
    private readonly ContainerBuilder ContainerBuilder;
    private readonly ContainerMutator ContainerMutator;
    public NodeType Type => NodeType.Row;

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