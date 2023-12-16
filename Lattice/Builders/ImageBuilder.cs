using Lattice.AttributeMutators;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export<IContainerBuilder>]
public class ImageBuilder(ContainerMutator ContainerMutator) : IContainerBuilder
{
    public NodeType Type => NodeType.Image;

    public void Build(Node node, IContainer container)
    {
        var newContainer = ContainerMutator.Mutate(container, node);
        var source = node.GetAttribute("src");
        if (!File.Exists(source)) return;
        var image = newContainer.Image(source);
        ScaleImage(node, image);
    }
    
    public void ScaleImage(Node node, ImageDescriptor image)
    {
        var scaling = node.GetAttribute("scaling");
        switch (scaling)
        {
            case "area":
                image.FitArea();
                break;
            case "height":
                image.FitHeight();
                break;
            case "unproportionate":
            case "freeform":
                image.FitUnproportionally();
                break;
            default:
                AutoScale(node, image);
                break;
        }
    }
    /// <summary>
    /// Automatically scales the image based on the dimension attributes of the node.
    /// </summary>
    /// <param name="node">The node to read attributes from.</param>
    /// <param name="imageDescriptor">The image to apply scaling to..</param>
    public void AutoScale(Node node, ImageDescriptor imageDescriptor)
    {
        var hasWidth = node.GetAttribute("width") != null;
        var hasHeight = node.GetAttribute("height") != null;

        if (hasWidth && hasHeight)
        {
            imageDescriptor.FitUnproportionally();
        }
        else if (hasWidth)
        {
            imageDescriptor.FitWidth();
        }
        else if (hasHeight)
        {
            imageDescriptor.FitHeight();
        }
    }
}