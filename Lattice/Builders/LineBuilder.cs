using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export<IContainerBuilder>]
public class LineHorizontalBuilder(
    ColourConverter ColourConverter,
    ContainerMutator ContainerMutator
)
    : IContainerBuilder
{
    public NodeType Type => NodeType.LineHorizontal;

    public void Build(Node node, IContainer container)
    {
        var newContainer = ContainerMutator.Mutate(container, node);

        var width = node.GetAttributeFloat("lineWidth");
        var line = newContainer.LineHorizontal(width ?? 1);
        var colour = node.GetAttribute("colour");
        if (colour is not null)
        {
            line.LineColor(ColourConverter.ConvertToHex(colour));
        }
    }
}

[Export<IContainerBuilder>]
public class LineVerticalBuilder : IContainerBuilder
{
    public NodeType Type => NodeType.LineVertical;

    public LineVerticalBuilder(ColourConverter colourConverter, ContainerMutator containerMutator)
    {
        ColourConverter = colourConverter;
        ContainerMutator = containerMutator;
    }

    private ColourConverter ColourConverter;
    private readonly ContainerMutator ContainerMutator;

    public void Build(Node node, IContainer container)
    {
        var newContainer = ContainerMutator.Mutate(container, node);
        var width = node.GetAttributeFloat("lineWidth");
        
        var line = newContainer.LineVertical(width ?? 1);
        var colour = node.GetAttribute("colour");
        if (colour is not null)
        {
            line.LineColor(ColourConverter.ConvertToHex(colour));
        }
    }
}