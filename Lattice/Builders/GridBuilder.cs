using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export(typeof(IContainerBuilder))]
public class GridBuilder : IContainerBuilder
{
    private readonly ContainerBuilder ContainerBuilder;
    private readonly ContainerMutator ContainerMutator;

    public NodeType Type => NodeType.Grid;

    public GridBuilder(ContainerBuilder containerBuilder, ContainerMutator containerMutator)
    {
        ContainerBuilder = containerBuilder;
        ContainerMutator = containerMutator;
    }

    public void Build(Node node, IContainer container)
    {
        var widths = (node.GetAttribute("widths") ?? "")
            .Split(",")
            .Select(t => int.TryParse(t, out var width) ? width : 1);

        ContainerMutator
            .Mutate(container, node)
            .Grid(grid =>
            {
                var spacing = node.GetAttributeFloat("spacing");
                var horizontalSpacing = node.GetAttributeFloat("horizontalSpacing");
                var verticalSpacing = node.GetAttributeFloat("verticalSpacing");
                if (spacing.HasValue)
                {
                    grid.Spacing(spacing.Value);
                }

                if (verticalSpacing.HasValue)
                {
                    grid.VerticalSpacing(verticalSpacing.Value);
                }

                if (horizontalSpacing.HasValue)
                {
                    grid.HorizontalSpacing(horizontalSpacing.Value);
                }
                var i = 0;
                ContainerBuilder.Build(node, () =>
                {
                    var size = widths.ElementAtOrDefault(i);
                    i++;
                    return grid.Item(size);
                });
            });
    }
}