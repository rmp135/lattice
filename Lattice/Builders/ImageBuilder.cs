using Lattice.AttributeMutators;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export(typeof(IContainerBuilder))]
public class ImageBuilder : IContainerBuilder
{
    private readonly ContainerMutator ContainerMutator;
    public NodeType Type => NodeType.Image;
    public ImageBuilder(ContainerMutator containerMutator)
    {
        ContainerMutator = containerMutator;
    }

    public void Build(Node node, IContainer container)
    {
        var newContainer = ContainerMutator.Mutate(container, node);
        var source = node.GetAttribute("src");
        if (source is null) return;
        
        var scaling = node.GetAttribute("scaling");
        var imageScaling = scaling switch
        {
            "area" => ImageScaling.FitArea,
            "height" => ImageScaling.FitHeight,
            _ => ImageScaling.FitWidth
        };
        newContainer.Image(source, imageScaling);
    }
}